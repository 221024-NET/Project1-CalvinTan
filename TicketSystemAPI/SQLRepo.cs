using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace TicketSystemAPI
{
    public class SQLRepo
    {
        public List<Employee> getAll(string connString)
        {
            var roster = new List<Employee>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append("SELECT * FROM TicketSystemDB.Employees");
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //creates a new employee and adds to list
                    Employee employee = new Employee();
                    employee.userName = reader["UserName"].ToString();
                    employee.password = reader["Password"].ToString();
                    employee.role = reader["Role"].ToString();
                    employee.iD = reader.GetInt32(0);
                    roster.Add(employee);
                }
                reader.Close();
                cmd.Dispose();
            }

            return roster;
        }

        public Employee getEmployee(int id,string connString)
        {
            Employee employee = new Employee();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"SELECT * FROM TicketSystemDB.Employees WHERE EmployeeId = {id}");
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    employee.userName = reader["UserName"].ToString();
                    employee.password = reader["Password"].ToString();
                    employee.role = reader["Role"].ToString();
                    employee.iD = reader.GetInt32(0);

                }
                reader.Close();
                cmd.Dispose();
            }
            return employee;
        }

        //maybe try calling usernameCheck in here to check for duplicate username
        public Employee? insertEmployee(Employee employee,string connString)
        {
            using(SqlConnection connection = new SqlConnection(connString))
            {

                //if the username is unique, this part inserts the employee into db
                StringBuilder qry = new StringBuilder();
                qry.Append($"INSERT INTO TicketSystemDB.Employees " +
                    $"(UserName,Password)" +
                    $"VALUES ('{employee.userName}','{employee.password}')");
                qry.Append("SELECT @@IDENTITY;");
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                

                int newID = Convert.ToInt32(cmd.ExecuteScalar());
                employee.iD = newID;
                cmd.Dispose();
            }
            return employee;
        }
        
        public void updateEmployee(Employee employee,int id,string connString)
        {
            using(SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"UPDATE TicketSystemDB.Employees " +
                    $"SET UserName = '{employee.userName}'," +
                    $"Password = '{employee.password}'," +
                    $"Role = '{employee.role}' " +
                    $"WHERE EmployeeId = {id};"
                    );
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return;
        }

        public void deleteEmployee(string connString, int id)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append(" DELETE FROM TicketSystemDB.Employees");
                qry.Append($" WHERE EmployeeId = {id}");
                Console.WriteLine(qry.ToString());

                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return;
        }

        public List<Ticket> seeOwnTickets(string connString,int id)
        {
            var tickets = new List<Ticket>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"SELECT * FROM TicketSystemDB.Tickets WHERE SubmittedBy = {id};");
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Ticket ticket = new Ticket();
                    ticket.Id = reader.GetInt32(0);
                    ticket.amount = reader.GetInt32(1);
                    ticket.status = reader["Status"].ToString();
                    ticket.description = reader["Description"].ToString();
                    ticket.submittedBy = reader.GetInt32(4);
                    int? processedBy = reader.IsDBNull(5) ? null : reader.GetInt32(5);
                    ticket.processedBy = processedBy;
                    tickets.Add(ticket);
                }
                reader.Close();
                cmd.Dispose();

            }
            return tickets;
        }

        //Make return type nullable so i can return null
        public Employee? login(string connString,string userName,string password)
        {
            Employee employee = new Employee();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"SELECT * FROM TicketSystemDB.Employees WHERE UserName = '{userName}' AND Password = '{password}'");
                Console.WriteLine(qry.ToString());

                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        employee.userName = reader["UserName"].ToString();
                        employee.password = reader["Password"].ToString();
                        employee.iD = reader.GetInt32(0);
                        employee.role = reader["Role"].ToString();
                        break;
                    } 
                reader.Close();
                cmd.Dispose();
            }
            if (employee.iD != 0)
                return employee;
            else
                return null;
             
            
        }

        //manager methods
        public List<Ticket> getAllPendingTickets(string connString)
        {
           var tickets = new List<Ticket>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append("SELECT * FROM TicketSystemDB.Tickets WHERE Status = 'pending';");
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    
                    Ticket ticket = new Ticket();
                    ticket.Id = reader.GetInt32(0);
                    ticket.amount = reader.GetInt32(1);
                    ticket.status = reader["Status"].ToString();
                    ticket.description = reader["Description"].ToString();
                    ticket.submittedBy = reader.GetInt32(4);
                    int? processedBy = reader.IsDBNull(5) ? null : reader.GetInt32(5);
                    ticket.processedBy = processedBy;
                    tickets.Add(ticket);
                }
                reader.Close();
                cmd.Dispose();
            }
            return tickets;
        }

        public void updateTicket(int managerId,int ticketId,string status,string connString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"UPDATE TicketSystemDB.Tickets SET Status = '{status}'," +
                    $"ProcessedBy = {managerId} WHERE TicketId = {ticketId};");
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return;
        }
        
        public Ticket insertTicket(int employeeID,Ticket ticket,string connString)
        {
            ticket.submittedBy = employeeID;
            using(SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"INSERT INTO TicketSystemDB.Tickets (Amount,Description,SubmittedBy) VALUES" +
                    $"({ticket.amount},'{ticket.description}',{employeeID})");
                qry.Append("SELECT @@IDENTITY");
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                int newID = Convert.ToInt32(cmd.ExecuteScalar());
                ticket.Id = newID;
                cmd.Dispose();
            }
            return ticket;
        }
        

        //if name is taken, returns false;
        public bool userNameCheck(string username,string connString)
        {
            using(SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"SELECT * FROM TicketSystemDB.Employees WHERE UserName = '{username}'");
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    return false;
                else
                    return true;

            }
        }

        public Ticket getTicket(int ticketId,string connString)
        {
            Ticket ticket = new Ticket();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"SELECT * FROM TicketSystemDB.Tickets WHERE TicketId = {ticketId}");
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ticket.Id = reader.GetInt32(0);
                    ticket.amount = reader.GetInt32(1);
                    ticket.status = reader["Status"].ToString();
                    ticket.description = reader["Description"].ToString();
                    ticket.submittedBy = reader.GetInt32(4);
                    int? processedBy = reader.IsDBNull(5) ? null : reader.GetInt32(5);
                    ticket.processedBy = processedBy;
                }
            }
            return ticket;
        }


    }
}

