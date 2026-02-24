import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { ChevronRight, Home, Plus, Minus, ShoppingBag, Heart, Share2 } from 'lucide-react';
import api from '../services/api';
import type { ApiResponse, Product } from '../types';
import ProductCard from '../components/products/ProductCard';

const ProductDetails = () => {
    const { id } = useParams<{ id: string }>();
    const [product, setProduct] = useState<Product | null>(null);
    const [relatedProducts, setRelatedProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [quantity, setQuantity] = useState(1);
    const [selectedSize, setSelectedSize] = useState('Medium');
    const [activeTab, setActiveTab] = useState('description');

    useEffect(() => {
        const fetchProduct = async () => {
            setLoading(true);
            try {
                const response = await api.get<ApiResponse<Product>>(`/v1/products/${id}`);
                if (response.data.success) {
                    setProduct(response.data.data);

                    // Fetch related products (same category)
                    const relatedResponse = await api.get<ApiResponse<any>>('/v1/products', {
                        params: { categoryId: response.data.data.categoryId, pageSize: 4 }
                    });
                    if (relatedResponse.data.success) {
                        setRelatedProducts(relatedResponse.data.data.items.filter((p: any) => p.id !== id));
                    }
                }
            } catch (error) {
                console.error('Failed to fetch product details:', error);
            } finally {
                setLoading(false);
            }
        };

        if (id) fetchProduct();
    }, [id]);

    if (loading) {
        return (
            <div className="min-h-screen bg-white py-20 px-6">
                <div className="max-w-7xl mx-auto flex flex-col md:flex-row gap-16 animate-pulse">
                    <div className="md:w-1/2 aspect-square bg-coffee-50" />
                    <div className="md:w-1/2 space-y-8">
                        <div className="h-4 bg-coffee-50 w-24" />
                        <div className="h-12 bg-coffee-50 w-3/4" />
                        <div className="h-6 bg-coffee-50 w-32" />
                        <div className="h-32 bg-coffee-50 w-full" />
                        <div className="h-16 bg-coffee-50 w-full" />
                    </div>
                </div>
            </div>
        );
    }

    if (!product) {
        return (
            <div className="min-h-screen flex flex-col items-center justify-center text-center p-6">
                <h2 className="text-4xl font-display text-coffee-800 mb-4">Ối! Sản phẩm không tồn tại</h2>
                <p className="text-coffee-400 mb-8 uppercase tracking-widest text-xs">Có vẻ như sản phẩm này đã được thưởng thức hết sạch rồi.</p>
                <Link to="/products" className="btn-primary">Quay lại thực đơn</Link>
            </div>
        );
    }

    return (
        <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="bg-white"
        >
            {/* Breadcrumb */}
            <div className="bg-coffee-50 border-b border-coffee-100 py-8 px-6 md:px-12">
                <div className="max-w-7xl mx-auto flex items-center gap-4 text-[10px] uppercase tracking-[0.2em] text-coffee-400 font-bold">
                    <Link to="/" className="hover:text-coffee-700 transition-colors flex items-center gap-2">
                        <Home className="w-3 h-3" /> Trang chủ
                    </Link>
                    <ChevronRight className="w-3 h-3" />
                    <Link to="/products" className="hover:text-coffee-700 transition-colors">Sản phẩm</Link>
                    <ChevronRight className="w-3 h-3" />
                    <span className="text-coffee-800 border-b border-coffee-800">{product.name}</span>
                </div>
            </div>

            <main className="max-w-7xl mx-auto px-6 md:px-12 py-20">
                <div className="flex flex-col lg:flex-row gap-20">
                    {/* Image Section */}
                    <div className="lg:w-1/2">
                        <div className="sticky top-32">
                            <motion.div
                                layoutId={`product-image-${product.id}`}
                                className="aspect-[4/5] bg-coffee-50 overflow-hidden relative group"
                            >
                                <img
                                    src={product.image || 'https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?q=80&w=2070&auto=format&fit=crop'}
                                    alt={product.name}
                                    className="w-full h-full object-cover transition-transform duration-1000 group-hover:scale-105"
                                />
                                <div className="absolute top-6 right-6 flex flex-col gap-4">
                                    <button className="bg-white/80 backdrop-blur-md p-3 rounded-full text-coffee-900 hover:bg-red-500 hover:text-white transition-all shadow-lg">
                                        <Heart className="w-5 h-5" />
                                    </button>
                                    <button className="bg-white/80 backdrop-blur-md p-3 rounded-full text-coffee-900 hover:bg-coffee-700 hover:text-white transition-all shadow-lg">
                                        <Share2 className="w-5 h-5" />
                                    </button>
                                </div>
                            </motion.div>
                        </div>
                    </div>

                    {/* Content Section */}
                    <div className="lg:w-1/2 space-y-12">
                        <div className="space-y-4">
                            <span className="text-[10px] uppercase tracking-[0.4em] text-coffee-400 font-bold">
                                {product.categoryName}
                            </span>
                            <h1 className="text-5xl md:text-6xl font-display text-coffee-800 leading-tight">
                                {product.name}
                            </h1>
                            <p className="text-3xl font-light text-coffee-600">
                                {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(product.price)}
                            </p>
                        </div>

                        <div className="space-y-8">
                            {/* Size Selection */}
                            <div className="space-y-4">
                                <span className="text-[10px] uppercase tracking-widest text-coffee-900 font-bold">Chọn Kích Cỡ</span>
                                <div className="flex gap-4">
                                    {['Small', 'Medium', 'Large'].map((size) => (
                                        <button
                                            key={size}
                                            onClick={() => setSelectedSize(size)}
                                            className={`px-8 py-3 text-xs uppercase tracking-widest transition-all duration-300 border ${selectedSize === size
                                                ? 'bg-coffee-800 text-white border-coffee-800'
                                                : 'text-coffee-400 border-coffee-100 hover:border-coffee-400'
                                                }`}
                                        >
                                            {size}
                                        </button>
                                    ))}
                                </div>
                            </div>

                            {/* Quantity & Actions */}
                            <div className="flex flex-col sm:flex-row gap-6 pt-8 border-t border-coffee-50">
                                <div className="flex items-center border border-coffee-100 h-16">
                                    <button
                                        onClick={() => setQuantity(Math.max(1, quantity - 1))}
                                        className="px-6 hover:text-coffee-600 transition-colors"
                                    >
                                        <Minus className="w-4 h-4" />
                                    </button>
                                    <span className="w-12 text-center font-display text-xl">{quantity}</span>
                                    <button
                                        onClick={() => setQuantity(quantity + 1)}
                                        className="px-6 hover:text-coffee-600 transition-colors"
                                    >
                                        <Plus className="w-4 h-4" />
                                    </button>
                                </div>
                                <button className="flex-1 bg-coffee-800 text-white h-16 px-12 uppercase tracking-[0.2em] text-[10px] font-bold hover:bg-coffee-900 transition-all flex items-center justify-center gap-4 group">
                                    <ShoppingBag className="w-4 h-4 transition-transform group-hover:-translate-y-1" />
                                    Thêm vào giỏ hàng
                                </button>
                            </div>
                        </div>

                        {/* Tabs */}
                        <div className="pt-12 border-t border-coffee-50">
                            <div className="flex gap-12 mb-8 border-b border-coffee-50">
                                {['description', 'details'].map((tab) => (
                                    <button
                                        key={tab}
                                        onClick={() => setActiveTab(tab)}
                                        className={`pb-4 text-[10px] uppercase tracking-widest font-bold transition-all ${activeTab === tab
                                            ? 'text-coffee-800 border-b-2 border-coffee-800'
                                            : 'text-coffee-300 hover:text-coffee-500'
                                            }`}
                                    >
                                        {tab === 'description' ? 'Mô tả' : 'Thông tin thêm'}
                                    </button>
                                ))}
                            </div>
                            <div className="text-coffee-600 font-light leading-relaxed min-h-[100px]">
                                {activeTab === 'description' ? (
                                    <p>{product.description || 'Một thức uống tuyệt vời được pha chế tỉ mỉ từ những hạt cà phê rang xay nguyên chất, mang lại hương vị đặc trưng quyến rũ.'}</p>
                                ) : (
                                    <ul className="space-y-4">
                                        <li className="flex justify-between text-xs">
                                            <span className="text-coffee-400 uppercase tracking-widest">Phân loại</span>
                                            <span className="text-coffee-800 font-medium">{product.categoryName}</span>
                                        </li>
                                        <li className="flex justify-between text-xs">
                                            <span className="text-coffee-400 uppercase tracking-widest">Năng lượng</span>
                                            <span className="text-coffee-800 font-medium">180 kcal</span>
                                        </li>
                                        <li className="flex justify-between text-xs">
                                            <span className="text-coffee-400 uppercase tracking-widest">Thành phần</span>
                                            <span className="text-coffee-800 font-medium text-right max-w-[200px]">Cà phê Arabica, Sữa tươi, Đá viên cao cấp</span>
                                        </li>
                                    </ul>
                                )}
                            </div>
                        </div>
                    </div>
                </div>

                {/* Related Products */}
                {relatedProducts.length > 0 && (
                    <section className="mt-32 pt-20 border-t border-coffee-100">
                        <div className="flex flex-col md:flex-row justify-between items-end mb-16 gap-8">
                            <div className="space-y-4">
                                <span className="text-[10px] uppercase tracking-[0.4em] text-coffee-400 font-bold">Khám phá</span>
                                <h2 className="text-4xl md:text-5xl font-display text-coffee-800">Sản phẩm liên quan</h2>
                            </div>
                        </div>
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-x-8 gap-y-16">
                            {relatedProducts.map((p) => (
                                <ProductCard key={p.id} {...p} />
                            ))}
                        </div>
                    </section>
                )}
            </main>

            {/* Sticky Info for Mobile */}
            <div className="md:hidden fixed bottom-6 left-6 right-6 bg-coffee-900 text-white p-4 flex items-center justify-between shadow-2xl z-50 rounded-full">
                <div className="pl-4">
                    <p className="text-[10px] uppercase tracking-widest opacity-60">Tổng cộng</p>
                    <p className="font-display text-lg">{new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(product.price * quantity)}</p>
                </div>
                <button className="bg-white text-coffee-900 px-6 py-3 rounded-full text-[10px] uppercase tracking-widest font-bold">
                    Mua ngay
                </button>
            </div>
        </motion.div>
    );
};

export default ProductDetails;
