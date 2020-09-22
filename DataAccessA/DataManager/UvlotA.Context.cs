﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccessA.DataManager
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class UvlotAEntities : DbContext
    {
        public UvlotAEntities()
            : base("name=UvlotAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccomodationType> AccomodationTypes { get; set; }
        public virtual DbSet<ApplicationStatu> ApplicationStatus { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<Bankold> Bankolds { get; set; }
        public virtual DbSet<BanksManager> BanksManagers { get; set; }
        public virtual DbSet<DocUpload> DocUploads { get; set; }
        public virtual DbSet<EmployerLoanDetail> EmployerLoanDetails { get; set; }
        public virtual DbSet<EmploymentStatu> EmploymentStatus { get; set; }
        public virtual DbSet<Guarantor> Guarantors { get; set; }
        public virtual DbSet<Institution> Institutions { get; set; }
        public virtual DbSet<InstitutionType> InstitutionTypes { get; set; }
        public virtual DbSet<LGA> LGAs { get; set; }
        public virtual DbSet<LoanApplication> LoanApplications { get; set; }
        public virtual DbSet<LoanApproval> LoanApprovals { get; set; }
        public virtual DbSet<LoanInterestRate> LoanInterestRates { get; set; }
        public virtual DbSet<LoanLedger> LoanLedgers { get; set; }
        public virtual DbSet<LoanProduct> LoanProducts { get; set; }
        public virtual DbSet<LoansLedger> LoansLedgers { get; set; }
        public virtual DbSet<LoanTenure> LoanTenures { get; set; }
        public virtual DbSet<LoanType> LoanTypes { get; set; }
        public virtual DbSet<MaritalStatu> MaritalStatus { get; set; }
        public virtual DbSet<MarketingChannel> MarketingChannels { get; set; }
        public virtual DbSet<MarketingDetail> MarketingDetails { get; set; }
        public virtual DbSet<MeansOfIdentification> MeansOfIdentifications { get; set; }
        public virtual DbSet<MonthVal> MonthVals { get; set; }
        public virtual DbSet<NigerianState> NigerianStates { get; set; }
        public virtual DbSet<NYSCApplicationStatu> NYSCApplicationStatus { get; set; }
        public virtual DbSet<NyscLoanApplication> NyscLoanApplications { get; set; }
        public virtual DbSet<NYSCLoanApproval> NYSCLoanApprovals { get; set; }
        public virtual DbSet<NYSCLoanLedger> NYSCLoanLedgers { get; set; }
        public virtual DbSet<NYSCLoanSetUp> NYSCLoanSetUps { get; set; }
        public virtual DbSet<NyscMaritalStatu> NyscMaritalStatus { get; set; }
        public virtual DbSet<NYSCReferralLedger> NYSCReferralLedgers { get; set; }
        public virtual DbSet<NYSCRelative> NYSCRelatives { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PageAuthentication> PageAuthentications { get; set; }
        public virtual DbSet<pageHeader> pageHeaders { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PatnerTransactLog> PatnerTransactLogs { get; set; }
        public virtual DbSet<PaymentFlag> PaymentFlags { get; set; }
        public virtual DbSet<RepaymentMethod> RepaymentMethods { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<StudentLoanDetail> StudentLoanDetails { get; set; }
        public virtual DbSet<StudentRecord> StudentRecords { get; set; }
        public virtual DbSet<Title> Titles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<A_OfflineApplications> A_OfflineApplications { get; set; }
        public virtual DbSet<AA_Sheet> AA_Sheet { get; set; }
        public virtual DbSet<Ledger> Ledgers { get; set; }
        public virtual DbSet<LoanSerialNo> LoanSerialNoes { get; set; }
        public virtual DbSet<Repayment> Repayments { get; set; }
    
        public virtual ObjectResult<BorroweredLoans_Result> BorroweredLoans()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<BorroweredLoans_Result>("BorroweredLoans");
        }
    
        public virtual ObjectResult<DisbursedLoans_Result> DisbursedLoans()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DisbursedLoans_Result>("DisbursedLoans");
        }
    
        public virtual ObjectResult<GetNYSCDefaultLoans_Result> GetNYSCDefaultLoans()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetNYSCDefaultLoans_Result>("GetNYSCDefaultLoans");
        }
    
        public virtual ObjectResult<GetNYSCLoanApplicationSummary_Result> GetNYSCLoanApplicationSummary()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetNYSCLoanApplicationSummary_Result>("GetNYSCLoanApplicationSummary");
        }
    
        public virtual ObjectResult<GetReferralActivity_Result> GetReferralActivity()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetReferralActivity_Result>("GetReferralActivity");
        }
    
        public virtual ObjectResult<GetReferrals_Result> GetReferrals(Nullable<int> roleID)
        {
            var roleIDParameter = roleID.HasValue ?
                new ObjectParameter("RoleID", roleID) :
                new ObjectParameter("RoleID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetReferrals_Result>("GetReferrals", roleIDParameter);
        }
    
        public virtual ObjectResult<LoanDueForDebit_Result> LoanDueForDebit(Nullable<System.DateTime> date)
        {
            var dateParameter = date.HasValue ?
                new ObjectParameter("Date", date) :
                new ObjectParameter("Date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoanDueForDebit_Result>("LoanDueForDebit", dateParameter);
        }
    
        public virtual ObjectResult<LoanRepayment_Result> LoanRepayment(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoanRepayment_Result>("LoanRepayment", startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<LoanTransactionbyDate_Result> LoanTransactionbyDate(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoanTransactionbyDate_Result>("LoanTransactionbyDate", startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<NyscApplicationRelated_Result> NyscApplicationRelated(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<NyscApplicationRelated_Result>("NyscApplicationRelated", startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<OutStandingLoan_Result> OutStandingLoan(Nullable<System.DateTime> endDate)
        {
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<OutStandingLoan_Result>("OutStandingLoan", endDateParameter);
        }
    
        public virtual ObjectResult<ReferralAgentPerformance_Result> ReferralAgentPerformance()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReferralAgentPerformance_Result>("ReferralAgentPerformance");
        }
    
        public virtual ObjectResult<Repayment_Result> Repayment(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Repayment_Result>("Repayment", startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<RevenueEarned_Result> RevenueEarned(Nullable<System.DateTime> endDate)
        {
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RevenueEarned_Result>("RevenueEarned", endDateParameter);
        }
    
        public virtual ObjectResult<RevenueReceived_Result> RevenueReceived(Nullable<System.DateTime> endDate)
        {
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RevenueReceived_Result>("RevenueReceived", endDateParameter);
        }
    
        public virtual ObjectResult<Top50ReferralPerformance_Result> Top50ReferralPerformance()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Top50ReferralPerformance_Result>("Top50ReferralPerformance");
        }
    
        public virtual int UpdatePartner(Nullable<double> myTimer, string myTokenVal, string partnerID, Nullable<int> recID)
        {
            var myTimerParameter = myTimer.HasValue ?
                new ObjectParameter("myTimer", myTimer) :
                new ObjectParameter("myTimer", typeof(double));
    
            var myTokenValParameter = myTokenVal != null ?
                new ObjectParameter("myTokenVal", myTokenVal) :
                new ObjectParameter("myTokenVal", typeof(string));
    
            var partnerIDParameter = partnerID != null ?
                new ObjectParameter("partnerID", partnerID) :
                new ObjectParameter("partnerID", typeof(string));
    
            var recIDParameter = recID.HasValue ?
                new ObjectParameter("RecID", recID) :
                new ObjectParameter("RecID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdatePartner", myTimerParameter, myTokenValParameter, partnerIDParameter, recIDParameter);
        }
    }
}
