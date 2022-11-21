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
        {
            //RunAsync().GetAwaiter().GetResult();
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
            {
                Employee e = new Employee(1, "Today", "Is", "So", "Hot");
                var url = await CreateEmployeeAsync(e);
                Console.WriteLine("Employee URL is: " + url);

                e = await GetEmployeeAsync(url.ToString());
                displayEmployee(e);
            } 
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

        static void displayEmployee(Employee employee)
        {
            Console.WriteLine($"ID: {employee.iD}\t " +
                $"First Name: {employee.fname}\t " +
                $"Last Name: {employee.lname}\t " +
                $"Username: {employee.userName}\t " +
                $"Password: {employee.password}\t " +
                $"Role: {employee.role}\t "
                );
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
        /*
        static async Task<Employee> PromptLogin(string userName,string passWord)
        {

            Console.WriteLine("Welcome to the system");
            Console.WriteLine("Please enter your username");
            string user = Console.ReadLine();
            Console.WriteLine("Please enter your password");
            string password = Console.ReadLine();
            
            
        }
        */
        
        //Manager methods
        

}
}

