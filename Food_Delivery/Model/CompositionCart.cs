﻿using Food_Delivery.Model.DPO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    public class CompositionCart
    {
        public int id { get; set; }

        public int shoppingCartId { get; set; }

        public int dishesId { get; set; }

        public int quantity { get; set; }

        // устанваливаем внешний ключ на таблицу ShoppingCart
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

        // устанваливаем внешний ключ на таблицу Dishes
        public virtual Dishes Dishes { get; set; } = null!;

        public CompositionCart() { }

        public CompositionCart(int id, int shoppingCartId, int dishesId, int quantity, ShoppingCart shoppingCart, Dishes dishes)
        {
            this.id = id;
            this.shoppingCartId = shoppingCartId;
            this.dishesId = dishesId;
            this.quantity = quantity;
            ShoppingCart = shoppingCart;
            Dishes = dishes;
        }

        // получаем товар из корзины у CompositionCartDPO
        private async Task<CompositionCart> CompositionCartDPO(CompositionCartDPO compositionCartDPO)
        {
            CompositionCart compositionCart = new CompositionCart();

            compositionCart.id = compositionCartDPO.id;
            compositionCart.shoppingCartId = compositionCartDPO.shoppingCartId;
            compositionCart.dishesId = compositionCartDPO.dishesId;
            if(quantity != null)
            {
                compositionCart.quantity = quantity;
            }

            return compositionCart;
        }
    }
}
