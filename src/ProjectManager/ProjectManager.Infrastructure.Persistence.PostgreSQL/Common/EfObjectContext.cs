using ProjectManager.Core.SeedWork.Domain;
using ProjectManager.Infrastructure.Persistence.PostgreSQL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Persistence.PostgreSQL.Common
{
    /// <summary>
    /// Represents base object context
    /// </summary>
    public abstract class EfObjectContext : DbContext, IUnitOfWork
    {
        private readonly IDbExceptionParserProvider _exceptionParser;
        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction GetCurrentTransaction()
        {
            return _currentTransaction;
        }

        public bool HasActiveTransaction => _currentTransaction != null;

        protected EfObjectContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions) { }

        protected EfObjectContext(DbContextOptions dbContextOptions, IDbExceptionParserProvider exceptionParser)
            : base(dbContextOptions)
        {
            _exceptionParser = exceptionParser ?? throw new ArgumentNullException(nameof(exceptionParser));
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return null;
            }

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (transaction != _currentTransaction)
            {
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
            }

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public new virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                // it will throw always
                await GetFullErrorTextAndRollbackEntityChanges(ex);
                throw;
            }
        }

        /// <summary>
        /// Rollback of entity changes and return full error message
        /// </summary>
        /// <param name="exception">AppException</param>
        /// <returns>Error message</returns>
        public virtual async Task GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {
            //rollback entity changes
            if (this is DbContext dbContext)
            {
                try
                {
                    var entries = dbContext.ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                    entries.ForEach(entry => entry.State = EntityState.Unchanged);
                }
                catch (Exception ex)
                {
                    exception = new DbUpdateException(exception.ToString(), ex);
                }
            }
            // save previous state
            await base.SaveChangesAsync();

            _exceptionParser.ParseAndRaise(exception);
        }
    }
}