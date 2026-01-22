using Core.Models.Cart;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICartService
{
	Task AddUpdateAsync(CartAddUpdateModel model);

	Task<List<CartItemModel>> GetListAsync();

	Task<List<CartItemModel>> GetAllTransportationsAsync();
}