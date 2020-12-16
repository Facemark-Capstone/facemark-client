// David Wahid
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using model.Extensions;

namespace model.Controllers
{
    [Route("/internal-api/model")]
    public class ModelApiController : Controller
    {
        public ModelApiController()
        {
        }

        [HttpPost]
        [Route("analyze")]
        public async Task<IActionResult> Analyze()
        {
            string image = "image.jpg";

            try
            {
                using (var stream = System.IO.File.Create(image))
                {
                    await Request.Body.CopyToAsync(stream);
                    if (stream.Length < 1)
                    {
                        return BadRequest("Request body is empty!");
                    }
                }
                var result = "./run.sh".Bash();

                if (result.Length < 1)
                {
                    return StatusCode(500);
                }

                if (System.IO.File.Exists(image))
                {
                    System.IO.File.Delete(image);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
