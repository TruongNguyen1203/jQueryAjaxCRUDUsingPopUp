using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JqueryAjaxCRUDPopUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static JqueryAjaxCRUDPopUp.Helper;

namespace JqueryAjaxCRUDPopUp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly TransactionDbContext _context;

        public TransactionController(TransactionDbContext context)
        {
            _context = context;
        }
       public async Task<IActionResult> Index()
        {
            return View(await _context.Transaction.ToListAsync());
        }

        // GET: Transaction/AddOrEdit 
        // GET: Transaction/AddOrEdit/5
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id=0)
        {
            if (id==0)
                return View(new TransactionModel());
            else
            {
                var transactionModel = await _context.Transaction.FindAsync(id);
                if(transactionModel == null )
                {
                    return NotFound();
                }
                return View(transactionModel);
            }
        }

        //POST: Transaction/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("TransactionId,AccountNumber,BeneficiaryName,BankName,SWIFTCode,Amount,Date")] TransactionModel transactionModel)
        {
            if (ModelState.IsValid)
            {
                if(id == 0)
                {
                    transactionModel.Date = DateTime.Now;
                    _context.Add(transactionModel);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    try
                    {
                        _context.Update(transactionModel);
                        await _context.SaveChangesAsync();

                    }
                    catch(DbUpdateConcurrencyException)
                    {
                        return NotFound();
                    }
                  
                }

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Transaction.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", transactionModel) });
        }
       
        
        //POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactionModel = await _context.Transaction.FindAsync(id);
            _context.Transaction.Remove(transactionModel);
            await _context.SaveChangesAsync();

            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Transaction.ToList()) });
        }
        
    }
}
