using System;
using System.IO;
using System.Threading.Tasks;
using EasyGames.Web.Services;
using Xunit;

namespace EasyGames.Tests
{
    // Very simple test: make sure SendAsync writes something meaningful to console.
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendAsync_Writes_To_Console()
        {
            // arrange
            var svc = new EmailService();
            var sw = new StringWriter();
            var oldOut = Console.Out;
            Console.SetOut(sw);

            try
            {
                // act
                await svc.SendAsync("test@example.com", "Hello", "Thanks for ordering!");

                // assert
                var output = sw.ToString();
                Assert.Contains("test@example.com", output);
                Assert.Contains("Hello", output);
                Assert.Contains("Thanks for ordering!", output);
            }
            finally
            {
                // put the console back
                Console.SetOut(oldOut);
            }
        }
    }
}



