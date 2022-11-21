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
                    employee.fname = reader["FirstName"].ToString();
                    employee.lname = reader["LastName"].ToString();
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
                    employee.fname = reader["FirstName"].ToString();
                    employee.lname = reader["LastName"].ToString();
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

        public Employee insertEmployee(Employee employee,string connString)
        {
            using(SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"INSERT INTO TicketSystemDB.Employees " +
                    $"(FirstName,LastName,UserName,Password)" +
                    $"VALUES ('{employee.fname}','{employee.lname}','{employee.userName}','{employee.password}')");
                qry.Append("SELECT @@IDENTITY;");
                Console.WriteLine(qry.ToString());
                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();

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
                    $"SET FirstName = '{employee.fname}'," +
                    $"LastName = '{employee.lname}'," +
                    $"UserName = '{employee.userName}'," +
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

        public Employee login(string connString,string userName,string password)
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
                try
                {
                    while (reader.Read())
                    {
                        
                        employee.fname = reader["FirstName"].ToString();
                        employee.lname = reader["LastName"].ToString();
                        employee.userName = reader["UserName"].ToString();
                        employee.password = reader["Password"].ToString();
                        employee.iD = reader.GetInt32(0);
                        employee.role = reader["Role"].ToString();
                        break;
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                reader.Close();
                cmd.Dispose();
            }
            return employee;
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

        public void updateTicket(Employee manager,int ticketId,string status,string connString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append($"UPDATE TicketSystemDB.Tickets SET Status = '{status}'," +
                    $"ProcessedBy = {manager.iD} WHERE TicketId = {ticketId};");
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

    }
}

