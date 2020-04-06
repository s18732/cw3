using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            Log log = new Log();
            var metoda = httpContext.Request.Method;
            log.Dodaj("Metoda: " + metoda);
            var sciezka = httpContext.Request.Path;
            log.Dodaj("Sciezka: " + sciezka);
            //var cialo = httpContext.Request.Body;
            string bodyStr = "";
            using (StreamReader reader
             = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStr = await reader.ReadToEndAsync();
                //httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                httpContext.Request.Body.Position = 0;
            }
            log.Dodaj("Cialo: " + bodyStr);
            var query = httpContext.Request.QueryString.ToString();
            log.Dodaj("Query: " + query);
            //Our code
            log.DodajBezDaty("");
            await _next(httpContext);
        }
    }
    public class Log
    {
        public void Dodaj(string logMessage)
        {
            try
            {
                using (StreamWriter plik = File.AppendText("log.txt"))
                {
                    plik.WriteLine("[" +DateTime.Now +"]: "+logMessage);
                }
            }
            catch (Exception)
            {
                throw new FileNotFoundException("Blad zapisu do pliku log.txt");
            }
        }
        public void DodajBezDaty(string logMessage)
        {
            try
            {
                using (StreamWriter plik = File.AppendText("log.txt"))
                {
                    plik.WriteLine(logMessage);
                }
            }
            catch (Exception)
            {
                throw new FileNotFoundException("Blad zapisu do pliku log.txt");
            }
        }
    }
}
