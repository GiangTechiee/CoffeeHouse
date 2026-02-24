import { motion, AnimatePresence } from 'framer-motion';
import { ShoppingBag, Trash2, Plus, Minus, ArrowRight, ChevronLeft, Home } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';

const Cart = () => {
    const { cart, removeFromCart, addToCart, totalPrice, totalItems } = useCart();
    const navigate = useNavigate();

    if (cart.length === 0) {
        return (
            <div className="min-h-[70vh] flex flex-col items-center justify-center p-6 text-center">
                <motion.div
                    initial={{ scale: 0.8, opacity: 0 }}
                    animate={{ scale: 1, opacity: 1 }}
                    className="w-32 h-32 bg-coffee-50 rounded-full flex items-center justify-center mb-8"
                >
                    <ShoppingBag className="w-12 h-12 text-coffee-200" />
                </motion.div>
                <h2 className="text-4xl font-display text-coffee-800 mb-4">Giỏ hàng trống</h2>
                <p className="text-coffee-400 mb-12 uppercase tracking-[0.2em] text-[10px] font-bold">Hãy chọn những thức uống bạn yêu thích!</p>
                <Link to="/products" className="btn-primary">Quay lại thực đơn</Link>
            </div>
        );
    }

    return (
        <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="bg-white min-h-screen"
        >
            {/* Breadcrumb */}
            <div className="bg-coffee-50 border-b border-coffee-100 py-8 px-6 md:px-12">
                <div className="max-w-7xl mx-auto flex items-center gap-4 text-[10px] uppercase tracking-[0.2em] text-coffee-400 font-bold">
                    <Link to="/" className="hover:text-coffee-700 transition-colors flex items-center gap-2">
                        <Home className="w-3 h-3" /> Trang chủ
                    </Link>
                    <ChevronLeft className="w-3 h-3" />
                    <span className="text-coffee-800 border-b border-coffee-800">Giỏ hàng</span>
                </div>
            </div>

            <main className="max-w-7xl mx-auto px-6 md:px-12 py-20">
                <div className="flex flex-col lg:flex-row gap-20">
                    {/* Items List */}
                    <div className="lg:flex-1">
                        <div className="flex justify-between items-end mb-12 pb-8 border-b border-coffee-50">
                            <h1 className="text-4xl md:text-5xl font-display text-coffee-800">Giỏ hàng của bạn</h1>
                            <span className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold">{totalItems} sản phẩm</span>
                        </div>

                        <div className="space-y-12">
                            <AnimatePresence>
                                {cart.map((item) => (
                                    <motion.div
                                        key={item.id}
                                        layout
                                        initial={{ opacity: 0, x: -20 }}
                                        animate={{ opacity: 1, x: 0 }}
                                        exit={{ opacity: 0, x: -50 }}
                                        className="flex flex-col sm:flex-row items-center gap-8 group"
                                    >
                                        <div className="w-32 aspect-square overflow-hidden bg-coffee-50 shrink-0">
                                            <img src={item.image} alt={item.name} className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-700" />
                                        </div>

                                        <div className="flex-1 space-y-2 text-center sm:text-left">
                                            <h3 className="font-display text-2xl text-coffee-800">{item.name}</h3>
                                            <p className="text-xs uppercase tracking-widest text-coffee-400">Kích cỡ: Medium</p>
                                        </div>

                                        <div className="flex items-center border border-coffee-100 h-12 shrink-0">
                                            <button className="px-4 hover:text-coffee-600 transition-colors">
                                                <Minus className="w-3 h-3" />
                                            </button>
                                            <span className="w-8 text-center font-display text-sm">{item.quantity}</span>
                                            <button
                                                onClick={() => addToCart(item)}
                                                className="px-4 hover:text-coffee-600 transition-colors"
                                            >
                                                <Plus className="w-3 h-3" />
                                            </button>
                                        </div>

                                        <div className="w-32 text-center sm:text-right">
                                            <p className="font-display text-lg text-coffee-800">
                                                {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(item.price * item.quantity)}
                                            </p>
                                        </div>

                                        <button
                                            onClick={() => removeFromCart(item.id)}
                                            className="text-coffee-200 hover:text-red-500 transition-colors p-2"
                                        >
                                            <Trash2 className="w-5 h-5" />
                                        </button>
                                    </motion.div>
                                ))}
                            </AnimatePresence>
                        </div>

                        <div className="mt-16">
                            <Link to="/products" className="inline-flex items-center gap-4 text-[10px] uppercase tracking-widest font-bold text-coffee-400 hover:text-coffee-800 transition-colors">
                                <ChevronLeft className="w-4 h-4" /> Tiếp tục chọn món
                            </Link>
                        </div>
                    </div>

                    {/* Summary */}
                    <div className="lg:w-96">
                        <div className="bg-coffee-50 p-10 sticky top-32">
                            <h2 className="font-display text-2xl text-coffee-800 mb-8 pb-4 border-b border-coffee-100">Tổng cộng</h2>

                            <div className="space-y-6 mb-8">
                                <div className="flex justify-between text-xs">
                                    <span className="text-coffee-400 uppercase tracking-widest font-bold">Tạm tính</span>
                                    <span className="text-coffee-800 font-bold">{new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(totalPrice)}</span>
                                </div>
                                <div className="flex justify-between text-xs">
                                    <span className="text-coffee-400 uppercase tracking-widest font-bold">Phí vận chuyển</span>
                                    <span className="text-coffee-800 font-bold">Miễn phí</span>
                                </div>
                            </div>

                            <div className="flex justify-between items-end mb-10 pt-6 border-t border-coffee-100">
                                <span className="text-[10px] uppercase tracking-[0.2em] text-coffee-400 font-bold">Thanh giữ món</span>
                                <span className="text-3xl font-display text-coffee-800">{new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(totalPrice)}</span>
                            </div>

                            <button
                                onClick={() => navigate('/checkout')}
                                className="w-full bg-coffee-800 text-white h-16 uppercase tracking-[0.2em] text-[10px] font-bold hover:bg-coffee-900 transition-all flex items-center justify-center gap-4 group"
                            >
                                Tiến hành thanh toán
                                <ArrowRight className="w-4 h-4 transition-transform group-hover:translate-x-1" />
                            </button>

                            <div className="mt-8 flex items-center gap-4 text-coffee-400 opacity-60">
                                <div className="p-2 border border-coffee-200 rounded-full shrink-0">
                                    <Home className="w-4 h-4" />
                                </div>
                                <p className="text-[10px] uppercase tracking-widest leading-relaxed">
                                    Đảm bảo giao hàng trong 30 phút hoặc nhận voucher miễn phí.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </main>
        </motion.div>
    );
};

export default Cart;
