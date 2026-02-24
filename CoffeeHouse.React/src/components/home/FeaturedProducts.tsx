import { useEffect, useState } from 'react';
import ProductCard from '../products/ProductCard';
import api from '../../services/api';
import type { ApiResponse, PaginatedResponse, Product } from '../../types';

const FeaturedProducts = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchFeaturedProducts = async () => {
            try {
                const response = await api.get<ApiResponse<PaginatedResponse<Product>>>('/v1/products', {
                    params: { pageSize: 4 }
                });
                if (response.data.success) {
                    setProducts(response.data.data.items);
                }
            } catch (error) {
                console.error('Failed to fetch featured products:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchFeaturedProducts();
    }, []);

    if (loading) {
        return (
            <section className="py-24 px-6 md:px-12 bg-coffee-50/50">
                <div className="max-w-7xl mx-auto">
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-x-8 gap-y-16">
                        {[1, 2, 3, 4].map((i) => (
                            <div key={i} className="animate-pulse">
                                <div className="aspect-[4/5] bg-coffee-100 mb-6"></div>
                                <div className="h-4 bg-coffee-100 w-2/3 mb-4"></div>
                                <div className="h-4 bg-coffee-100 w-1/3"></div>
                            </div>
                        ))}
                    </div>
                </div>
            </section>
        );
    }

    return (
        <section className="py-24 px-6 md:px-12 bg-coffee-50/50">
            <div className="max-w-7xl mx-auto">
                <div className="flex flex-col md:flex-row justify-between items-end mb-16 gap-8">
                    <div className="space-y-4">
                        <span className="text-[10px] uppercase tracking-[0.4em] text-coffee-400 font-bold">Menu Gợi ý</span>
                        <h2 className="text-4xl md:text-5xl font-display text-coffee-800">Sản phẩm nổi bật</h2>
                    </div>
                    <button className="text-sm uppercase tracking-widest text-coffee-600 hover:text-coffee-900 transition-colors border-b border-coffee-200 pb-1">
                        Xem tất cả thực đơn
                    </button>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-x-8 gap-y-16">
                    {products.map((product) => (
                        <ProductCard key={product.id} {...product} />
                    ))}
                </div>
            </div>
        </section>
    );
};

export default FeaturedProducts;
