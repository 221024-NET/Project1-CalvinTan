namespace TicketSystemAPI
{
    public class Employee
    {
        public int iD { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        
        public string role { get; set; }

        public Employee(int id, string first_name, string last_name, string username, string password,string role = "employee")
        {
            this.iD = id;
            this.fname = first_name;
            this.lname = last_name;
            this.userName = username;
            this.password = password;
            this.role = role;
        }

        public Employee()
        {

        }

        public void sayHello()
        {
            Console.WriteLine("MY name is "+this.fname);
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
