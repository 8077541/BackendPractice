using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;

        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioController(
            UserManager<AppUser> userManager,
            IStockRepository stockRepository,
            IPortfolioRepository portfolioRepository
        )
        {
            _portfolioRepository = portfolioRepository;
            _userManager = userManager;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepository.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                return NotFound();
            }
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

            if (userPortfolio.Any(s => s.Symbol.ToLower() == symbol.ToLower()))
                return BadRequest("Stock already in portfolio");

            var portfolioModel = new Portfolio { AppUserId = appUser.Id, StockId = stock.Id };
            await _portfolioRepository.CreateAsync(portfolioModel);
            if (portfolioModel == null)
            {
                return BadRequest("Failed to add stock to portfolio");
            }
            else
            {
                return Created();
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

            var filteredStock = userPortfolio
                .Where(s => s.Symbol.ToLower() == symbol.ToLower())
                .ToList();
            if (filteredStock.Count == 1)
            {
                await _portfolioRepository.DeleteAsync(appUser, symbol);
            }
            else
            {
                return BadRequest("Stock not found in portfolio");
            }
            return Ok();
        }
    }
}
