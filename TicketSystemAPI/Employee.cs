namespace TicketSystemAPI
{
    public class Employee
    {
        public int iD { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        
        public string role { get; set; }

        public Employee(int id, string username, string password,string role = "employee")
        {
            this.iD = id;
            this.userName = username;
            this.password = password;
            this.role = role;
        }

        public Employee(string userName,string password,string role = "employee")
        {
            this.userName = userName;
            this.password = password;
            this.role = role;
        }

        public Employee()
        {

        }

        public bool isManager()
        {
            if (this.role == "manager")
                return true;
            else 
                return false;
        }
    }
}
