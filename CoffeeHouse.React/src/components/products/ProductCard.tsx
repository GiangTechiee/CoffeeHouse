import React from 'react';
import { Plus, Heart } from 'lucide-react';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';
import type { Product } from '../../types';

interface ProductCardProps extends Partial<Product> {
    // Keep standard props for backward compatibility if needed, but primarily use Product
}

const ProductCard: React.FC<ProductCardProps> = ({ id, name, price, image, imageUrl, categoryName }) => {
    return (
        <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="group relative"
        >
            <Link to={`/products/${id}`}>
                <div className="aspect-[3/4] overflow-hidden bg-coffee-50 relative mb-6">
                    <img
                        src={imageUrl || image || 'https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?q=80&w=2070&auto=format&fit=crop'}
                        alt={name}
                        className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
                    />

                    {/* Overlay Actions */}
                    <div className="absolute inset-0 bg-coffee-900/10 opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
                </div>
            </Link>

            <div className="absolute bottom-[108px] left-4 right-4 flex justify-between items-center translate-y-8 opacity-0 group-hover:translate-y-0 group-hover:opacity-100 transition-all duration-500">
                <button className="bg-white text-coffee-900 p-3 hover:bg-coffee-700 hover:text-white transition-colors shadow-xl">
                    <Plus className="w-5 h-5" />
                </button>
                <button className="bg-white text-coffee-900 p-3 hover:bg-red-500 hover:text-white transition-colors shadow-xl">
                    <Heart className="w-5 h-5" />
                </button>
            </div>

            <div className="space-y-2">
                <p className="text-[10px] uppercase tracking-[0.2em] text-coffee-400 font-semibold">
                    {categoryName || 'Sản phẩm'}
                </p>
                <Link to={`/products/${id}`}>
                    <h3 className="font-display text-xl text-coffee-800 group-hover:text-coffee-600 transition-colors">
                        {name}
                    </h3>
                </Link>
                <p className="text-coffee-600 font-medium">
                    {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price || 0)}
                </p>
            </div>
        </motion.div>
    );
};

export default ProductCard;
