// David Wahid
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace shared.Models.Analysis
{
    public class AnalyzeModel
    {
        public string UserId { get; set; }

        public string HubConnectionId { get; set; }

        public IFormFile Image { get; set; }
    }
}
