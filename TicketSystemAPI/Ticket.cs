namespace TicketSystemAPI
{
    public class Ticket
    {
        public int Id { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public string status { get; set; }  
        public int submittedBy { get; set; }
        public int? processedBy { get; set; }

        public Ticket(int id,int amount, string description, int submittedBy, int? processedBy, string status = "pending")
        {   
            this.Id = id; 
            this.amount = amount;
            this.description = description;
            this.status = status;
            this.submittedBy = submittedBy;
            this.processedBy = processedBy;
        }
        public Ticket()
        {

        }

        public Ticket(int amount,string description,int submittedBy)
        {
            this.amount=amount;
            this.description = description;
            this.submittedBy = submittedBy;
        }
    }
}
