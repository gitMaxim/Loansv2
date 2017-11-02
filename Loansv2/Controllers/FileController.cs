using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Loansv2.DAL;
using Loansv2.Models;

namespace Loansv2.Controllers
{
    public class FileController : Controller
    {
        private LoansContext _db = new LoansContext();


        public ActionResult Index(int id)
        {
            var fileToRetrieve = _db.Files.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }


        public ActionResult Create(int? loanAgreementId)
        {
            if (loanAgreementId == null)
                return HttpNotFound();

            var file = new File
            {
                LoanAgreementId = (int) loanAgreementId
            };

            return View(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "LoanAgreementId,FileName,ContentType,Content,FileType")] File file, 
            HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var f = new File
                    {
                        FileName = System.IO.Path.GetFileName(upload.FileName),
                        ContentType = upload.ContentType,
                        LoanAgreementId = file.LoanAgreementId
                    };
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        f.Content = reader.ReadBytes(upload.ContentLength);
                    }

                    _db.Files.Add(f);
                    _db.SaveChanges();
                }

                return RedirectToAction("Details", "LoanAgreement", new { id = file.LoanAgreementId });
            }

            return View(file);
        }

        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var file = _db.Files.Find(id);
            if (file == null)
                return concurrencyError.GetValueOrDefault()
                    ? (ActionResult)RedirectToAction("Index", "LoanAgreement")
                    : HttpNotFound();

            if (concurrencyError.GetValueOrDefault())
                ViewBag.ConcurrencyErrorMessage = GlobalMessages.ConcurrencyDelete;

            return View(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(File file)
        {
            int loanAgreementId = file.LoanAgreementId;
            try
            {
                _db.Entry(file).State = EntityState.Deleted;
                _db.SaveChanges();

                return RedirectToAction("Details", "LoanAgreement", new { id = loanAgreementId });
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = file.Id });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, GlobalMessages.LaterOrAdmin);
                return View(file);
            }
        }


        #region Remote validations
        public JsonResult UniqueFileName(string fileName, int loanAgreementId)
        {
            var result = _db.Files.FirstOrDefault(f => f.LoanAgreementId == loanAgreementId && f.FileName == fileName) == null;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();

            base.Dispose(disposing);
        }
    }
}
