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
                  Employee employee = new Employee();
                  Ticket ticket = new Ticket();
                  List<Ticket> tickets = new List<Ticket>();
                  List<Employee> employees = new List<Employee>();

                while(endApp == false)
                {
                    Console.WriteLine("Welcome to the Ticket Reimbursment System");
                    Console.WriteLine();
                    Console.WriteLine("What would you like to do today?");
                    Console.WriteLine("1. Register as Employee \t 2. Login");
                    int input = Convert.ToInt32(Console.ReadLine());
                    if(input == 1)
                    {
                        //Register page
                        Console.WriteLine("Registration Page");
                        Console.WriteLine("Please enter a username");
                        string username = Console.ReadLine();
                        Console.WriteLine("Please enter a password");
                        string password = Console.ReadLine();
                        Employee registration = new Employee(0, username, password);
                        await CreateEmployeeAsync(registration);
                        //if CreateEmployee gets a dupe the id becomes 0, use that as a condition

                    }
                    else if(input == 2)
                    {
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
                            Console.WriteLine("display manager menu");
                        else
                            Console.WriteLine("display employee menu");

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
            Console.WriteLine($"Ticket ID: {ticket.Id}\t " +
                $"Amount: {ticket.amount}\t " +
                $"Status: {ticket.status}\t " + 
                $"Description: {ticket.description}\t +" +
                $"Submitted By: {ticket.submittedBy}\t "+ 
                $"Processed By: {ticket.processedBy}");
        }

        static void displayTicketsList(List<Ticket> tickets)
        {
            foreach(Ticket ticket in tickets)
                Console.WriteLine($"Ticket ID: {ticket.Id}\t " +
                $"Amount: {ticket.amount}\t " +
                $"Status: {ticket.status}\t " +
                $"Description: {ticket.description}\t " +
                $"Submitted By: {ticket.submittedBy}\t " +
                $"Processed By: {ticket.processedBy}");
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
            return ticketList;
        }
        

}
}

