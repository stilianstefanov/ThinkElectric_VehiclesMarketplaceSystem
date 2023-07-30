﻿namespace ThinkElectric.Web.ViewModels.Cart;

using static Common.GeneralApplicationConstants;

public class CartItemViewModel
{
    public CartItemViewModel()
    {
        Quantity = DefaultCartItemQuantity;
    }

    public string Id { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string Price { get; set; } = null!;

    public int Quantity { get; set; }
}
