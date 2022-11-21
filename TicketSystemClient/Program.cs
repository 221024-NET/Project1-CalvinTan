using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using TicketSystemAPI;

namespace TicketSystemClient{
    class Program
    {
        static HttpClient client = new HttpClient();

        public static void Main(string[] args)
        {   /*
            Employee dave = new Employee(1, "dave", "davidson", "daviduser", "password");
            Employee chester = new Employee(2, "chester", "chesterson", "chesteruser", "chesterpass");
            
            Ticket ticket = new Ticket(1, 2, "Des", 3,4);
            Ticket ticket2 = new Ticket(2, 3, "another", 3, 4);
            displayTicket(ticket);
            List<Ticket> list = new List<Ticket>();
            list.Add(ticket);
            list.Add(ticket2);
            list.Add(ticket2);
            list.Add(ticket2);
            list.Add(ticket2);
            displayTicketsList(list);
             */
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://localhost:7298/");
            //clears everything from header, "blank slate"
            client.DefaultRequestHeaders.Accept.Clear();
            //indicates API will be dealing with JSON format
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {     //blank objects to initialize 
                bool endApp = false;
                bool loggedIn = false;
                string position ="Welcome";
                
                  Employee employee = new Employee();
                  Ticket ticket = new Ticket();
                  List<Ticket> tickets = new List<Ticket>();
                  List<Employee> employees = new List<Employee>();
                


 
                while (endApp == false)
                {
                    switch (position)
                    { 
                        case "Welcome":
                            Console.WriteLine("\t \t \tWelcome to the Ticket Reimbursment System");
                            Console.WriteLine();
                            Console.WriteLine("\t \t \t What would you like to do today?");
                            Console.WriteLine("1. Register as Employee \t 2. Login \t 3. Quit Application");
                            int input = Convert.ToInt32(Console.ReadLine());
                            if (input == 1)
                                position = "Register";
                            else if (input == 2)
                                position = "Login";
                            else
                                position = "Quit";
                            break;
                        case "Login":
                            Console.WriteLine("\t \t \t Login Menu" );
                            //Login Page
                            while (loggedIn != true)
                            {
                                employee = await loginPrompt();
                                if (employee != null)
                                {
                                    Console.WriteLine($"{employee.userName}" + " has logged in");
                                    loggedIn = true;
                                }
                                else
                                    Console.WriteLine("Invalid credentials, please try again");
                            }

                            //Manager check
                            if (employee.isManager())
                                position = "Manager";
                            else
                                position = "Employee";
                            break;
                        case "Register":
                            //Register page
                            Console.WriteLine("\t \t \t Registration Page");
                            Console.WriteLine("\t \t Please enter a username");
                            string username = Console.ReadLine();
                            Console.WriteLine("\t \t Please enter a password");
                            string password = Console.ReadLine();
                            Employee registration = new Employee( username, password);
                            System.Uri uri = await CreateEmployeeAsync(registration);
                            if (uri==null)
                            {
                                Console.WriteLine("Username is already taken, please choose another");
                                break;
                            }
                            Console.WriteLine("You have successfully created your account, you will be redirected to Login Page");
                            position = "Login";
                            break;
                        case "Employee":
                            Console.WriteLine("\t \t \t Employee Menu");
                            Console.WriteLine(" \t \t What would you like to do? ");
                            Console.WriteLine("1. View all your tickets \t 2. Submit a new ticket \t 3.Log out");
                            input = Convert.ToInt32(Console.ReadLine());
                            if (input == 1)
                                position = "View Tickets";
                            else if(input ==2)
                                position = "Submit Ticket";
                            else if (input==3 )
                            {
                                position = "Log out";
                                break;
                            }
                            break;
                        case "View Tickets":
                            Console.WriteLine("\t \t \t Your Tickets: ");
                            displayTicketsList(await getEmployeeTickets(employee));
                            position = "Employee";
                            break;
                        case "Submit Ticket":
                            Console.WriteLine("Please enter ticket amount (in dollars)");
                            int amount = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Please provide a short description of the ticket");
                            string description = Console.ReadLine();
                            ticket = new Ticket(0,amount, description, employee.iD,null);
                            uri = await createTicketAsync(ticket,employee);
                            if (uri != null)
                                Console.WriteLine("Your ticket has been submitted, it will be reviewed by a manager shortly");
                            else
                                Console.WriteLine("There was an error with your request, please try again");
                            
                            position = "Employee";
                            break;
                        case "Manager":
                            Console.WriteLine(" \t \t \t Manager Menu");
                            Console.WriteLine("\t \t What would you like to do today?");
                            Console.WriteLine("1. View all pending tickets 2. Logout");
                            input = Convert.ToInt32(Console.ReadLine());
                            if (input == 1)
                                position = "View Pending";
                            else
                                position = "Log out";

                            break;
                        case "View Pending":
                            Console.WriteLine("\t \t \t Here are all the pending tickets: ");
                            Console.WriteLine();
                            displayTicketsList(await getPendingTickets());
                            Console.WriteLine("\t \t Which ticket would you like to update?");
                            int ticketId = Convert.ToInt32(Console.ReadLine());
                            displayTicket(await getSpecificTicket(ticketId));
                            Console.WriteLine(" \t \t  Do you want to approve this ticket? ");
                            Console.WriteLine("1. Yes \t 2. No \t 3. Back");
                            input = Convert.ToInt32(Console.ReadLine());
                            if (input == 1)
                            {
                                await updateTicket(employee.iD, ticketId, "approved");
                                Console.WriteLine("\t \t \t Ticket has been approved");
                            }

                            else if (input == 2)
                            {
                                await updateTicket(employee.iD, ticketId, "denied");
                                Console.WriteLine("\t \t \t Ticket has been denied");
                            }
                            else if(input == 3)
                            {
                                position = "Manager";
                                break;
                            }    
                                
                            break;
                        case "Log out":
                            Console.WriteLine("\t \t \t You have logged out");
                            employee = new Employee();
                            loggedIn = false;
                            position = "Welcome";
                            break;
                        case "Quit":
                            Console.WriteLine(" \t \t \t Have a nice day!");
                            endApp = true;
                            break;
                        default:
                            Console.WriteLine("Defaulted");
                            endApp = true;
                            break;
                    }
                   

                }




            } 
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

        static void displayEmployee(Employee employee)
        {
            Console.WriteLine($"ID: {employee.iD}\t " +
                $"Username: {employee.userName}\t " +
                $"Password: {employee.password}\t " +
                $"Role: {employee.role}\t "
                );
        }

        static void displayEmployeesList(List<Employee> employees)
        {
            foreach(Employee person in employees) 
                Console.WriteLine($"ID: {person.iD}\t " +
                $"Username: {person.userName}\t " +
                $"Password: {person.password}\t " +
                $"Role: {person.role}\t "
                );
        }
        
        static void displayTicket(Ticket ticket)
        {
            Console.WriteLine("------------");
            Console.WriteLine("Ticket ID: " + ticket.Id);
            Console.WriteLine("Amount: " + ticket.amount);
            Console.WriteLine("Status: " + ticket.status);
            Console.WriteLine("Description: " + ticket.description);
            Console.WriteLine("Submitted By: " + ticket.submittedBy);
            Console.WriteLine("Processed By: " + ticket.processedBy);
            Console.WriteLine("------------");
        }

        static void displayTicketsList(List<Ticket> tickets)
        {
            foreach (Ticket ticket in tickets)
            {
                Console.WriteLine("------------");
                Console.WriteLine("Ticket ID: " + ticket.Id);
                Console.WriteLine("Amount: " + ticket.amount);
                Console.WriteLine("Status: " + ticket.status);
                Console.WriteLine("Description: " + ticket.description);
                Console.WriteLine("Submitted By: " + ticket.submittedBy);
                Console.WriteLine("Processed By: " + ticket.processedBy);
            }
            Console.WriteLine("------------");

        }

        //takes a URL and returns the employee
        static async Task<Employee> GetEmployeeAsync(string path)
        {
            Employee employee = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                employee = await response.Content.ReadAsAsync<Employee>();
            }
            return employee;
        }

        

        //creates employee and returns a URL which is the location of employee
        static async Task<Uri> CreateEmployeeAsync(Employee employee)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "employees", employee);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        static async Task<Employee> loginPrompt()
        {
            Employee employee = new Employee();
            Console.WriteLine("Please enter your username");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter your password");
            string password = Console.ReadLine();

            HttpResponseMessage response = await client.GetAsync($"/login/{username}/{password}");
            if (response.IsSuccessStatusCode)
            {
                employee = await response.Content.ReadAsAsync<Employee>();
            }
            return employee;
        }
        

        
        static async Task<List<Ticket>> getEmployeeTickets(Employee employee)
        {
            List<Ticket> ticketList = new List<Ticket>();
            HttpResponseMessage response = await client.GetAsync($"/employeeTicket/{employee.iD}");
            if(response.IsSuccessStatusCode)
            {
                ticketList = await response.Content.ReadAsAsync<List<Ticket>>();
            }
            return ticketList;
        }

        static async Task<Uri> createTicketAsync(Ticket ticket,Employee employee)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                $"/tickets/{employee.iD}", ticket);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        static async Task<List<Ticket>> getPendingTickets()
        {
            List<Ticket> ticketList = new List<Ticket>();
            HttpResponseMessage response = await client.GetAsync($"/allPendingTickets");
            if(response.IsSuccessStatusCode)
            {
                ticketList = await response.Content.ReadAsAsync<List<Ticket>>();
            }
            return ticketList;
        }

        static async Task<Ticket> getSpecificTicket(int ticketId)
        {
            Ticket ticket = new Ticket();
            HttpResponseMessage response = await client.GetAsync($"/tickets/{ticketId}");
            if (response.IsSuccessStatusCode)
            {
                ticket = await response.Content.ReadAsAsync<Ticket>();
            }
            return ticket;
        }
        
        //void for now, might change to Ticket return type if needed
        static async Task<Ticket> updateTicket(int managerId,int ticketId,string status)
        {
            Ticket ticket = new Ticket();
            HttpResponseMessage response = await client.PutAsJsonAsync($"/tickets/{managerId}/{ticketId}/{status}",managerId);
            response.EnsureSuccessStatusCode();
            ticket = await response.Content.ReadAsAsync<Ticket>();
            return ticket;
        }

}
}

