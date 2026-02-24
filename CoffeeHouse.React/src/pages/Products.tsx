import { useEffect, useState } from 'react';
import { AnimatePresence } from 'framer-motion';
import { Filter, Search, ChevronRight, Home as HomeIcon } from 'lucide-react';
import ProductCard from '../components/products/ProductCard';
import { Link } from 'react-router-dom';
import api from '../services/api';
import type { ApiResponse, PaginatedResponse, Product, Category } from '../types';

const Products = () => {
    const [categories, setCategories] = useState<Category[]>([]);
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [categoriesLoading, setCategoriesLoading] = useState(true);
    const [activeCategoryId, setActiveCategoryId] = useState<number | null>(null);
    const [searchQuery, setSearchQuery] = useState('');
    const [debouncedSearch, setDebouncedSearch] = useState('');

    // Debounce search query
    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(searchQuery);
        }, 500);
        return () => clearTimeout(timer);
    }, [searchQuery]);

    // Fetch Categories
    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await api.get<ApiResponse<PaginatedResponse<Category>>>('/v1/categories');
                if (response.data.success) {
                    setCategories(response.data.data.items);
                }
            } catch (error) {
                console.error('Failed to fetch categories:', error);
            } finally {
                setCategoriesLoading(false);
            }
        };
        fetchCategories();
    }, []);

    // Fetch Products
    useEffect(() => {
        const fetchProducts = async () => {
            setLoading(true);
            try {
                const params: any = {
                    pageSize: 50, // Increase for now, will implement proper pagination later
                };
                if (activeCategoryId) params.categoryId = activeCategoryId;
                if (debouncedSearch) params.search = debouncedSearch;

                const response = await api.get<ApiResponse<PaginatedResponse<Product>>>('/v1/products', { params });
                if (response.data.success) {
                    setProducts(response.data.data.items);
                }
            } catch (error) {
                console.error('Failed to fetch products:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchProducts();
    }, [activeCategoryId, debouncedSearch]);

    return (
        <div className="bg-white min-h-screen">
            {/* Breadcrumb */}
            <div className="bg-coffee-50 border-b border-coffee-100 py-8 px-6 md:px-12">
                <div className="max-w-7xl mx-auto flex items-center gap-4 text-[10px] uppercase tracking-[0.2em] text-coffee-400 font-bold">
                    <Link to="/" className="hover:text-coffee-700 transition-colors flex items-center gap-2">
                        <HomeIcon className="w-3 h-3" /> Trang chủ
                    </Link>
                    <ChevronRight className="w-3 h-3" />
                    <span className="text-coffee-800 border-b border-coffee-800">Sản phẩm</span>
                </div>
            </div>

            <div className="max-w-7xl mx-auto px-6 md:px-12 py-20">
                <div className="flex flex-col lg:flex-row gap-20">

                    {/* Sidebar */}
                    <aside className="lg:w-64 shrink-0">
                        <div className="sticky top-32 space-y-12">
                            {/* Search */}
                            <div className="relative">
                                <input
                                    type="text"
                                    placeholder="Tìm sản phẩm..."
                                    className="input-field pl-10 text-xs"
                                    value={searchQuery}
                                    onChange={(e) => setSearchQuery(e.target.value)}
                                />
                                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-coffee-300" />
                            </div>

                            {/* Category Filter */}
                            <div>
                                <h4 className="font-display text-xl mb-8 flex items-center gap-3 decoration-coffee-200 underline underline-offset-8">
                                    <Filter className="w-4 h-4" /> Phân loại
                                </h4>
                                <div className="flex flex-col gap-4">
                                    <button
                                        onClick={() => setActiveCategoryId(null)}
                                        className={`text-left text-xs uppercase tracking-widest transition-all duration-300 ${activeCategoryId === null
                                            ? 'text-coffee-800 font-bold pl-4 border-l-2 border-coffee-800'
                                            : 'text-coffee-400 hover:text-coffee-600 hover:pl-2'
                                            }`}
                                    >
                                        Tất cả
                                    </button>
                                    {categoriesLoading ? (
                                        [1, 2, 3, 4].map(i => (
                                            <div key={i} className="h-4 w-2/3 bg-coffee-50 animate-pulse rounded" />
                                        ))
                                    ) : (
                                        categories.map(cat => (
                                            <button
                                                key={cat.id}
                                                onClick={() => setActiveCategoryId(cat.id)}
                                                className={`text-left text-xs uppercase tracking-widest transition-all duration-300 ${activeCategoryId === cat.id
                                                    ? 'text-coffee-800 font-bold pl-4 border-l-2 border-coffee-800'
                                                    : 'text-coffee-400 hover:text-coffee-600 hover:pl-2'
                                                    }`}
                                            >
                                                {cat.name}
                                            </button>
                                        ))
                                    )}
                                </div>
                            </div>

                            {/* Promo Widget */}
                            <div className="bg-coffee-900 p-8 text-white relative overflow-hidden group">
                                <div className="relative z-10">
                                    <p className="text-[10px] uppercase tracking-widest text-coffee-300 mb-4">Ưu đãi tuần này</p>
                                    <h5 className="font-display text-2xl mb-6 italic">Mua 2 Tặng 1 <br /> Cold Brew</h5>
                                    <button className="text-[10px] uppercase tracking-widest border-b border-white pb-1 hover:text-coffee-300 hover:border-coffee-300 transition-colors">
                                        Xem chi tiết
                                    </button>
                                </div>
                                <div className="absolute -bottom-10 -right-10 w-24 h-24 bg-white/5 rounded-full group-hover:scale-150 transition-transform duration-700" />
                            </div>
                        </div>
                    </aside>

                    {/* Product Grid */}
                    <main className="flex-1">
                        <div className="flex justify-between items-end mb-16">
                            <div>
                                <h2 className="text-4xl font-display text-coffee-800 mb-2">Thực đơn</h2>
                                <p className="text-xs uppercase tracking-widest text-coffee-400">
                                    Hiển thị {products.length} sản phẩm
                                </p>
                            </div>
                            <div className="hidden md:block">
                                <select className="bg-transparent border-none text-[10px] uppercase tracking-widest font-bold text-coffee-600 outline-none cursor-pointer">
                                    <option>Mới nhất</option>
                                    <option>Giá thấp đến cao</option>
                                    <option>Giá cao đến thấp</option>
                                </select>
                            </div>
                        </div>

                        {loading ? (
                            <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-x-8 gap-y-16">
                                {[1, 2, 3, 4, 5, 6].map((i) => (
                                    <div key={i} className="animate-pulse">
                                        <div className="aspect-[3/4] bg-coffee-50 mb-6" />
                                        <div className="h-4 bg-coffee-50 w-2/3 mb-4" />
                                        <div className="h-4 bg-coffee-50 w-1/3" />
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-x-8 gap-y-16">
                                <AnimatePresence mode="popLayout">
                                    {products.map((product) => (
                                        <ProductCard key={product.id} {...product} />
                                    ))}
                                </AnimatePresence>
                            </div>
                        )}

                        {!loading && products.length === 0 && (
                            <div className="py-20 text-center">
                                <p className="font-display text-2xl text-coffee-300 italic">
                                    Không tìm thấy sản phẩm phù hợp.
                                </p>
                            </div>
                        )}
                    </main>
                </div>
            </div>
        </div>
    );
};

export default Products;
