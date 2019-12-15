﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreAPI.Models;

namespace MobileStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly MobileStoreDBContext _context;

        public AdminController(MobileStoreDBContext context)
        {
            _context = context;
        }

        // GET: api/Admin
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<TblMobile>>> GetTblMobile()
        {
            var listMobile = await _context.TblMobile
                .Join(_context.TblBrand,
                m => m.BrandId,
                b => b.BrandId, (m, b) => new
                {
                    m.MobileId,
                    m.MobileName,
                    m.UnitPrice,
                    m.Photo,
                    m.Status,
                    m.CreateDate,
                    BrandName = b.Name,
                    Options = _context.TblOption
                    .Select(op => new
                    {
                        op.MobileId,
                        op.Ram,
                        op.Memory,
                        op.Color,
                        op.Quantity,
                        op.ExtraPrice
                    }).Where(op => op.MobileId == m.MobileId).ToList()
                }).Where(m => m.Status.Equals("Active")).ToListAsync();
            return Ok(listMobile);
        }

        // GET: api/Admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblMobile>> GetTblMobile(string id)
        {
            var tblMobile = await _context.TblMobile.FindAsync(id);

            if (tblMobile == null)
            {
                return NotFound();
            }

            return tblMobile;
        }


        // PUT: api/Admin/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblMobile(string id, TblMobile tblMobile)
        {
            if (id != tblMobile.MobileId)
            {
                return BadRequest();
            }

            _context.Entry(tblMobile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblMobileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Admin
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TblMobile>> PostTblMobile(TblMobile tblMobile)
        {
            _context.TblMobile.Add(tblMobile);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblMobileExists(tblMobile.MobileId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblMobile", new { id = tblMobile.MobileId }, tblMobile);
        }

        // DELETE: api/Admin/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TblMobile>> DeleteTblMobile(string id)
        {
            var tblMobile = await _context.TblMobile.FindAsync(id);
            if (tblMobile == null)
            {
                return NotFound();
            }

            _context.TblMobile.Remove(tblMobile);
            await _context.SaveChangesAsync();

            return tblMobile;
        }

        private bool TblMobileExists(string id)
        {
            return _context.TblMobile.Any(e => e.MobileId == id);
        }
    }
}
