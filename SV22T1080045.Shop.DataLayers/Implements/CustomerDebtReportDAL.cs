using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.DomainModels.Warehouse;
using SV22T1080045.Shop.DataLayers;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class CustomerDebtReportDAL : ICustomerDebtReportDAL
    {
        private readonly ShopDbContext _context;

        public CustomerDebtReportDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<CustomerDebtSummaryModel> GetDebtSummary(DateTime? startDate, DateTime? endDate, int? customerId = null, string? status = null)
        {
            var query = _context.CustomerDebts
                .AsNoTracking()
                .Where(d => !d.IsDeleted);

            if (customerId.HasValue)
                query = query.Where(d => d.CustomerId == customerId.Value);

            if (startDate.HasValue)
                query = query.Where(d => d.CreatedTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(d => d.CreatedTime <= endDate.Value);

            var debts = query.ToList();

            var result = debts.Select(d =>
            {
                var transactions = _context.CustomerDebtTransactions
                    .AsNoTracking()
                    .Where(t => t.CustomerDebtId == d.Id && !t.IsDeleted)
                    .ToList();

                var openingBalance = transactions
                    .Where(t => t.TransactionDate < (startDate ?? DateTime.MinValue))
                    .Sum(t => t.TransactionType == "DEBIT" ? t.Amount : -t.Amount);

                var debitAmount = transactions
                    .Where(t => t.TransactionDate >= (startDate ?? DateTime.MinValue) &&
                                t.TransactionDate <= (endDate ?? DateTime.MaxValue) &&
                                t.TransactionType == "DEBIT")
                    .Sum(t => t.Amount);

                var creditAmount = transactions
                    .Where(t => t.TransactionDate >= (startDate ?? DateTime.MinValue) &&
                                t.TransactionDate <= (endDate ?? DateTime.MaxValue) &&
                                t.TransactionType == "CREDIT")
                    .Sum(t => t.Amount);

                var paidAmount = transactions
                    .Where(t => t.TransactionType == "CREDIT" && t.Reference?.StartsWith("PAYMENT") == true)
                    .Sum(t => t.Amount);

                var closingBalance = openingBalance + debitAmount - creditAmount;

                var overdueAmount = 0m;
                if (DateTime.Today > d.DueDate)
                {
                    overdueAmount = closingBalance;
                }

                var debtStatus = "Pending";
                if (closingBalance <= 0)
                    debtStatus = "Paid";
                else if (paidAmount > 0)
                    debtStatus = "PartiallyPaid";
                else if (overdueAmount > 0)
                    debtStatus = "Overdue";

                if (!string.IsNullOrEmpty(status) && debtStatus != status)
                    return null;

                return new CustomerDebtSummaryModel
                {
                    CustomerId = d.CustomerId,
                    CustomerName = d.CustomerName ?? "",
                    CustomerPhone = "", // Customer phone not in CustomerDebt
                    OpeningBalance = openingBalance,
                    DebitAmount = debitAmount,
                    CreditAmount = creditAmount,
                    PaidAmount = paidAmount,
                    ClosingBalance = closingBalance,
                    OverdueAmount = overdueAmount,
                    Status = debtStatus
                };
            }).Where(m => m != null).ToList()!;

            return result;
        }

        public List<CustomerDebtTransaction> GetDebtLedger(int customerId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var debtIds = _context.CustomerDebts
                .AsNoTracking()
                .Where(d => d.CustomerId == customerId && !d.IsDeleted)
                .Select(d => d.Id)
                .ToList();

            var query = _context.CustomerDebtTransactions
                .AsNoTracking()
                .Where(t => debtIds.Contains(t.CustomerDebtId) && !t.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            return query
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.CreatedTime)
                .ToList();
        }

        public List<CustomerDebtAgingModel> GetDebtAging(DateTime asOfDate, int? customerId = null)
        {
            var query = _context.CustomerDebts
                .AsNoTracking()
                .Where(d => !d.IsDeleted);

            if (customerId.HasValue)
                query = query.Where(d => d.CustomerId == customerId.Value);

            var debts = query.ToList();

            var result = debts.Select(d =>
            {
                var transactions = _context.CustomerDebtTransactions
                    .AsNoTracking()
                    .Where(t => t.CustomerDebtId == d.Id && !t.IsDeleted)
                    .ToList();

                var currentBalance = transactions
                    .Sum(t => t.TransactionType == "DEBIT" ? t.Amount : -t.Amount);

                var notDue = 0m;
                var overdue1To30 = 0m;
                var overdue31To60 = 0m;
                var overdueOver60 = 0m;

                var dueDate = d.DueDate;
                var daysOverdue = (asOfDate - dueDate).Days;

                if (daysOverdue <= 0)
                    notDue = currentBalance;
                else if (daysOverdue <= 30)
                    overdue1To30 = currentBalance;
                else if (daysOverdue <= 60)
                    overdue31To60 = currentBalance;
                else
                    overdueOver60 = currentBalance;

                return new CustomerDebtAgingModel
                {
                    CustomerId = d.CustomerId,
                    CustomerName = d.CustomerName ?? "",
                    CustomerPhone = "", // Customer phone not in CustomerDebt
                    CurrentBalance = currentBalance,
                    NotDue = notDue,
                    Overdue1To30 = overdue1To30,
                    Overdue31To60 = overdue31To60,
                    OverdueOver60 = overdueOver60
                };
            }).ToList();

            return result;
        }

        public DebtOverviewModel GetDebtOverview(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.CustomerDebts
                .AsNoTracking()
                .Where(d => !d.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(d => d.CreatedTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(d => d.CreatedTime <= endDate.Value);

            var debts = query.ToList();

            var totalCustomers = debts.Select(d => d.CustomerId).Distinct().Count();
            var customersWithDebt = debts.Where(d => d.DebtAmount > 0).Select(d => d.CustomerId).Distinct().Count();

            var debtIds = debts.Select(d => d.Id).ToList();
            var transactions = _context.CustomerDebtTransactions
                .AsNoTracking()
                .Where(t => debtIds.Contains(t.CustomerDebtId) && !t.IsDeleted)
                .ToList();

            var totalDebt = transactions.Where(t => t.TransactionType == "DEBIT").Sum(t => t.Amount);
            var totalPaid = transactions.Where(t => t.TransactionType == "CREDIT").Sum(t => t.Amount);
            var totalCurrentBalance = totalDebt - totalPaid;

            var totalOverdue = debts
                .Where(d => DateTime.Now > d.DueDate)
                .Sum(d => d.DebtAmount);

            return new DebtOverviewModel
            {
                TotalCustomers = totalCustomers,
                CustomersWithDebt = customersWithDebt,
                TotalDebt = totalDebt,
                TotalPaid = totalPaid,
                TotalOverdue = totalOverdue,
                TotalCurrentBalance = totalCurrentBalance
            };
        }
    }
}
