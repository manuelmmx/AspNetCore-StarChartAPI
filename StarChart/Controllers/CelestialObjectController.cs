﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            celestialObject.Satellites = _context.CelestialObjects.Where(i => i.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(i => i.Name == name);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var co in celestialObjects)
            {
                co.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == co.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var co in celestialObjects)
            {
                co.Satellites = _context.CelestialObjects.Where(i => i.OrbitedObjectId == co.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")] //What this paraketer does?
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var co = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);
            if (co == null)
            {
                return NotFound();
            }

            co.Name = celestialObject.Name;
            co.OrbitalPeriod = celestialObject.OrbitalPeriod;
            co.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(co);
            _context.SaveChanges();

            return NoContent(); //Whit this? and not the new object?

        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var co = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);
            if (co == null)
            {
                return NotFound();
            }

            co.Name = name;

            _context.CelestialObjects.Update(co);
            _context.SaveChanges();

            return NoContent(); //Whit this? and not the new object?
        }


        [HttpDelete("{id}")]
        public IActionResult Delete (int id)
        {
            var cos = _context.CelestialObjects.Where(i => i.Id == id);
            //if (cos.Count() == 0)
            if (!cos.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(cos);
            _context.SaveChanges();

            return NoContent();
        }

    }
    
}
