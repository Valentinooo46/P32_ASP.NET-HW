using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interfaces;
using Core.Models.Cart;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Core.Services;

public class CartService(AppDbTransferContext appDbContext,
 IAuthService authService, IMapper mapper) : ICartService
{
 public async Task AddUpdateAsync(CartAddUpdateModel model)
 {
 var userId = await authService.GetUserIdAsync();
 var existingCart = await appDbContext.Carts
 .SingleOrDefaultAsync(c => c.UserId == userId && c.TransportationId == model.TransportationId);

 if (existingCart != null)
 {
 existingCart.CountTikets = model.Quantity;
 }
 else
 {
 var newCart = new CartEntity
 {
 UserId = userId,
 TransportationId = model.TransportationId,
 CountTikets = model.Quantity
 };
 appDbContext.Carts.Add(newCart);
 }

 await appDbContext.SaveChangesAsync();
 }

 public async Task<List<CartItemModel>> GetListAsync()
 {
 var userId = await authService.GetUserIdAsync();
 var cartItems = await appDbContext.Carts
 .Where(c => c.UserId == userId)
 .ProjectTo<CartItemModel>(mapper.ConfigurationProvider)
 .ToListAsync();

 return cartItems;
 }

 public async Task<List<CartItemModel>> GetAllTransportationsAsync()
 {
 int? userId = null;
 try
 {
 userId = await authService.GetUserIdAsync();
 }
 catch (UnauthorizedAccessException)
 {
 // user not authenticated - will return transportations with Quantity =0
 userId = null;
 }

 var cartsForUser = userId.HasValue
 ? appDbContext.Carts.Where(x => x.UserId == userId.Value)
 : appDbContext.Carts.Where(x => false);

 var query = from t in appDbContext.Transportations
 join c in cartsForUser
 on t.Id equals c.TransportationId into gj
 from cart in gj.DefaultIfEmpty()
 select new CartItemModel
 {
 Id = t.Id,
 Code = t.Code,
 FromCityName = t.FromCity.Name,
 FromCountryName = t.FromCity.Country.Name,
 ToCityName = t.ToCity.Name,
 ToCountryName = t.ToCity.Country.Name,
 DepartureTime = t.DepartureTime.ToString("dd.MM.yyyy HH:mm"),
 ArrivalTime = t.ArrivalTime.ToString("dd.MM.yyyy HH:mm"),
 SeatsTotal = t.SeatsTotal,
 SeatsAvailable = t.SeatsAvailable,
 StatusName = t.Status.Name,
 Quantity = cart != null ? cart.CountTikets : (short)0
 };

 var result = await query.ToListAsync();
 return result;
 }
}
