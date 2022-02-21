using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Webtonic.Data;
using Webtonic.Models;

namespace Webtonic.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDBContext _db;

        public StudentController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<StudentModel> objList = _db.Students;
            return View(objList);

        }

        //GET-AddNewStudent
        public IActionResult AddNewStudent()
        {

            return View();

        }

        //POST-AddNewStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNewStudent(StudentModel obj)
        {
            if (ModelState.IsValid)
            {
                _db.Students.Add(obj);
                _db.SaveChanges();
                TempData["Success"] = "Student added successfully";
                return RedirectToAction("Index");
            }

            return View(obj);

        }

        //Delete Student Section

        //GET-DeleteStudent
        [HttpGet]
        public IActionResult DeleteStudentGET(int? StudentID)
        {
            IEnumerable<StudentModel> objList = _db.Students;
            if (StudentID == null || StudentID == 0)
            {
                return NotFound();
            }
            var obj = _db.Students.Find(StudentID);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);

        }

        //POST-DeleteCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStudentPOST(int? StudentID)
        {
            var obj = _db.Students.Find(StudentID);
            if (obj == null)
            {
                return NotFound();
            }

            _db.Students.Remove(obj);
            _db.SaveChanges();
            TempData["Success"] = "Student information deleted successfully";
            return RedirectToAction("Index");

        }

        //Update Student Section

        //GET-UpdateStudent
        public IActionResult UpdateStudentGET(int? StudentID)
        {
            if (StudentID == null || StudentID == 0)
            {
                return NotFound();
            }
            var obj = _db.Students.Find(StudentID);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);

        }

        //POST-UpdateStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStudentPOST(StudentModel obj)
        {
            if (ModelState.IsValid)
            {
                _db.Students.Update(obj);
                _db.SaveChanges();

                TempData["Success"] = "Student information updated successfully";
                return RedirectToAction("Index");
            }

            return View(obj);

        }

        //CSV File read

        public IActionResult UploadData()
        {

            return View();

        }

        public async Task<ResponseViewModel<object>> UploadDataPost(IFormFile file)
        //public async Task<IActionResult> UploadData(IFormFile file)
        {
            //try
            //{
            var fileextension = Path.GetExtension(file.FileName);
            var filename = Guid.NewGuid().ToString() + fileextension;
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", filename);
            using (FileStream fs = System.IO.File.Create(filepath))
            {
                file.CopyTo(fs);
            }
            if (fileextension == ".csv")
            {
                using (var reader = new StreamReader(filepath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<FileModel>();
                    foreach (var record in records)
                    {

                        if (string.IsNullOrWhiteSpace(record.StudentNumber))
                        {
                            break;
                        }
                        StudentModel student;
                        student = _db.Students.Where(s => s.StudentNumber == record.StudentNumber).FirstOrDefault();

                        if (student == null)
                        {
                            student = new StudentModel();
                        }

                        student.StudentNumber = record.StudentNumber;
                        student.Firstname = record.Firstname;
                        student.Surname = record.Surname;
                        student.CourseCode = record.CourseCode;
                        student.CourseDescription = record.CourseDescription;
                        student.Grade = record.Grade;

                        if (student.StudentID == record.StudentID)
                            _db.Students.Update(student);
                        else
                            _db.Students.Add(student);
                    }
                    _db.SaveChanges();
                }
            }
            //return Pages(Index);
            //}
            else
            {
                return new ResponseViewModel<object>
                {
                    Status = false,
                    Message = "You can only add CSV file",
                    StatusCode = System.Net.HttpStatusCode.UnprocessableEntity.ToString()
                };
            }
            return new ResponseViewModel<object>
            {
                Status = true,
                Message = "Data Updated Successfully",
                StatusCode = System.Net.HttpStatusCode.OK.ToString()
            };
            //}

            //catch (Exception e)
            //{
            //    await _exceptionServices.CreateLog(e, null);
            //    throw e;
            //}
        }

        //private IActionResult View(Func<IActionResult> index)
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class ResponseViewModel<T>
    {
        public bool Status { get; internal set; }
        public string StatusCode { get; internal set; }
        public string Message { get; internal set; }
    }
}
