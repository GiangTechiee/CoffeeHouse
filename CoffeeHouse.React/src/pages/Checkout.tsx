import { useState, useEffect } from 'react';
import { ChevronLeft, ArrowRight, CreditCard, Truck, MapPin, Loader2 } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';
import type { ApiResponse } from '../types';

const Checkout = () => {
    const { cart, totalPrice, clearCart } = useCart();
    const { user } = useAuth();
    const navigate = useNavigate();

    const [stores, setStores] = useState<any[]>([]);
    const [selectedStore, setSelectedStore] = useState<number | null>(null);
    const [paymentMethod, setPaymentMethod] = useState('Cash');
    const [loading, setLoading] = useState(false);
    const [storesLoading, setStoresLoading] = useState(true);

    useEffect(() => {
        const fetchStores = async () => {
            try {
                const response = await api.get<ApiResponse<any>>('/v1/stores');
                if (response.data.success) {
                    setStores(response.data.data.items);
                    if (response.data.data.items.length > 0) {
                        setSelectedStore(response.data.data.items[0].id);
                    }
                }
            } catch (error) {
                console.error('Failed to fetch stores:', error);
            } finally {
                setStoresLoading(false);
            }
        };
        fetchStores();
    }, []);

    const handlePlaceOrder = async () => {
        if (!selectedStore) {
            alert('Vui lòng chọn cửa hàng');
            return;
        }

        setLoading(true);
        try {
            const orderCommand = {
                coffeeShopId: selectedStore,
                paymentMethod: paymentMethod,
                items: cart.map(item => ({
                    productId: parseInt(item.id),
                    productName: item.name,
                    quantity: item.quantity,
                    unitPrice: item.price
                }))
            };

            const response = await api.post<ApiResponse<any>>('/v1/orders', orderCommand);
            if (response.data.success) {
                const order = response.data.data;
                clearCart();
                navigate(`/order-success/${order.id}`, { state: { order } });
            }
        } catch (error: any) {
            console.error('Checkout failed:', error);
            alert(error.response?.data?.message || 'Có lỗi xảy ra khi đặt hàng');
        } finally {
            setLoading(false);
        }
    };

    if (cart.length === 0) {
        return (
            <div className="min-h-screen flex flex-col items-center justify-center p-6 text-center">
                <h2 className="text-4xl font-display text-coffee-800 mb-4">Ối! Giỏ hàng đang trống</h2>
                <Link to="/products" className="btn-primary">Quay lại chọn món</Link>
            </div>
        );
    }

    return (
        <div className="bg-white min-h-screen">
            {/* Breadcrumb */}
            <div className="bg-coffee-50 border-b border-coffee-100 py-8 px-6 md:px-12">
                <div className="max-w-7xl mx-auto flex items-center gap-4 text-[10px] uppercase tracking-[0.2em] text-coffee-400 font-bold">
                    <Link to="/cart" className="hover:text-coffee-700 transition-colors flex items-center gap-2">
                        <ChevronLeft className="w-3 h-3" /> Giỏ hàng
                    </Link>
                    <span className="text-coffee-300">/</span>
                    <span className="text-coffee-800 border-b border-coffee-800">Thanh toán</span>
                </div>
            </div>

            <main className="max-w-7xl mx-auto px-6 md:px-12 py-20">
                <div className="flex flex-col lg:flex-row gap-20">
                    <div className="lg:flex-1 space-y-20">
                        {/* Section 1: Customer Info */}
                        <section className="space-y-10">
                            <div className="flex items-center gap-4">
                                <span className="w-10 h-10 bg-coffee-800 text-white rounded-full flex items-center justify-center font-display text-lg">1</span>
                                <h2 className="text-3xl font-display text-coffee-800">Thông tin người nhận</h2>
                            </div>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-8 bg-coffee-50 p-10">
                                <div className="space-y-2">
                                    <span className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold">Họ tên</span>
                                    <p className="text-lg font-display text-coffee-800">{user?.name || 'Vui lòng đăng nhập'}</p>
                                </div>
                                <div className="space-y-2">
                                    <span className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold">Email</span>
                                    <p className="text-lg font-display text-coffee-800">{user?.email || 'N/A'}</p>
                                </div>
                            </div>
                        </section>

                        {/* Section 2: Store Selection */}
                        <section className="space-y-10">
                            <div className="flex items-center gap-4">
                                <span className="w-10 h-10 bg-coffee-800 text-white rounded-full flex items-center justify-center font-display text-lg">2</span>
                                <h2 className="text-3xl font-display text-coffee-800">Chọn cửa hàng</h2>
                            </div>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                {storesLoading ? (
                                    [1, 2].map(i => <div key={i} className="h-24 bg-coffee-50 animate-pulse" />)
                                ) : (
                                    stores.map(store => (
                                        <button
                                            key={store.id}
                                            onClick={() => setSelectedStore(store.id)}
                                            className={`p-6 text-left border-2 transition-all ${selectedStore === store.id
                                                ? 'border-coffee-800 bg-coffee-50'
                                                : 'border-coffee-100 hover:border-coffee-300'
                                                }`}
                                        >
                                            <div className="flex items-start gap-4">
                                                <MapPin className={`w-5 h-5 shrink-0 ${selectedStore === store.id ? 'text-coffee-800' : 'text-coffee-200'}`} />
                                                <div className="space-y-1">
                                                    <p className="font-display text-lg text-coffee-800">{store.name}</p>
                                                    <p className="text-xs text-coffee-400 leading-relaxed">{store.address}</p>
                                                </div>
                                            </div>
                                        </button>
                                    ))
                                )}
                            </div>
                        </section>

                        {/* Section 3: Payment Method */}
                        <section className="space-y-10">
                            <div className="flex items-center gap-4">
                                <span className="w-10 h-10 bg-coffee-800 text-white rounded-full flex items-center justify-center font-display text-lg">3</span>
                                <h2 className="text-3xl font-display text-coffee-800">Phương thức thanh toán</h2>
                            </div>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <button
                                    onClick={() => setPaymentMethod('Cash')}
                                    className={`p-6 flex items-center gap-4 border-2 transition-all ${paymentMethod === 'Cash'
                                        ? 'border-coffee-800 bg-coffee-50'
                                        : 'border-coffee-100 hover:border-coffee-300'
                                        }`}
                                >
                                    <Truck className="w-6 h-6 text-coffee-800" />
                                    <span className="text-xs uppercase tracking-widest font-bold text-coffee-800">Tiền mặt khi nhận hàng</span>
                                </button>
                                <button
                                    onClick={() => setPaymentMethod('Card')}
                                    className={`p-6 flex items-center gap-4 border-2 transition-all opacity-50 cursor-not-allowed ${paymentMethod === 'Card'
                                        ? 'border-coffee-800 bg-coffee-50'
                                        : 'border-coffee-100'
                                        }`}
                                    disabled
                                >
                                    <CreditCard className="w-6 h-6 text-coffee-200" />
                                    <span className="text-xs uppercase tracking-widest font-bold text-coffee-400">Thẻ ATM / Visa (Sắp ra mắt)</span>
                                </button>
                            </div>
                        </section>
                    </div>

                    {/* Order Summary Sidebar */}
                    <div className="lg:w-96">
                        <div className="bg-coffee-900 text-white p-10 sticky top-32">
                            <h2 className="font-display text-2xl mb-8 pb-4 border-b border-white/10 italic">Tóm tắt đơn hàng</h2>

                            <div className="space-y-8 mb-12 max-h-60 overflow-y-auto pr-4 custom-scrollbar">
                                {cart.map(item => (
                                    <div key={item.id} className="flex justify-between items-start gap-4">
                                        <div className="space-y-1">
                                            <p className="text-xs font-bold leading-tight">{item.name}</p>
                                            <p className="text-[10px] text-coffee-400">x{item.quantity} • Medium</p>
                                        </div>
                                        <span className="text-xs font-medium">
                                            {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(item.price * item.quantity)}
                                        </span>
                                    </div>
                                ))}
                            </div>

                            <div className="space-y-6 pt-8 border-t border-white/10">
                                <div className="flex justify-between text-[10px] uppercase tracking-widest text-coffee-400">
                                    <span>Tạm tính</span>
                                    <span>{new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(totalPrice)}</span>
                                </div>
                                <div className="flex justify-between text-[10px] uppercase tracking-widest text-coffee-400">
                                    <span>Phí giao hàng</span>
                                    <span>Miễn phí</span>
                                </div>
                                <div className="flex justify-between items-end pt-4">
                                    <span className="text-xs uppercase tracking-[0.2em] font-bold">Tổng thanh toán</span>
                                    <span className="text-3xl font-display">
                                        {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(totalPrice)}
                                    </span>
                                </div>
                            </div>

                            <button
                                onClick={handlePlaceOrder}
                                disabled={loading || storesLoading || !user}
                                className="w-full bg-white text-coffee-900 h-16 mt-12 uppercase tracking-[0.2em] text-[10px] font-bold hover:bg-coffee-100 transition-all flex items-center justify-center gap-4 group disabled:opacity-50"
                            >
                                {loading ? (
                                    <Loader2 className="w-4 h-4 animate-spin" />
                                ) : (
                                    <>
                                        Xác nhận đặt hàng
                                        <ArrowRight className="w-4 h-4 transition-transform group-hover:translate-x-1" />
                                    </>
                                )}
                            </button>

                            {!user && (
                                <p className="mt-6 text-[10px] text-center text-red-400 font-bold uppercase tracking-widest animate-pulse">
                                    Vui lòng đăng nhập để thanh toán
                                </p>
                            )}
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default Checkout;
