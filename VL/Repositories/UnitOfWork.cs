using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Video_Library_Api.Contexts;

namespace Video_Library_Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            bool saved = false;
            while(!saved)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    saved = true;
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        if(databaseValues != null)
                        {
                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = databaseValues[property];

                                // TODO: decide which value should be written to database
                                // proposedValues[property] = <value to be saved>;
                                if(property.Name == "ProcessesLeft")
                                {
                                    // proposedValue = (int) databaseValue - 1;
                                    Console.WriteLine($"prop: {proposedValue}, dat: {databaseValue}, orig: {entry.OriginalValues[property]}");
                                    proposedValues[property] = (int) databaseValue - 1;
                                }
                            }
                        }
                        
                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                }
            }
        }
    }
}