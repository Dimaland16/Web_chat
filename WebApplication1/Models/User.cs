﻿namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
    }
}
