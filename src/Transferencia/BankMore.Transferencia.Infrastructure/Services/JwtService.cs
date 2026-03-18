using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BankMore.Transferencia.Application.Services.Interfaces;

namespace BankMore.Transferencia.Infrastructure.Services
{
    public class JwtService(string secretKey, int expiracaoMinutos = 60) : IJwtService
    {
        private readonly string _secretKey = secretKey;
        private readonly int _expiracaoMinutos = expiracaoMinutos;

        public string GerarToken(int numeroConta)
        {
            // Claims — dados que ficam dentro do token            
            var claims = new[]
            {
                new Claim("numeroConta", numeroConta.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "BankMore",
                audience: "BankMore",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiracaoMinutos),
                signingCredentials: credenciais);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}