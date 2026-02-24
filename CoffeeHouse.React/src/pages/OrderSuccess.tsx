import { motion } from 'framer-motion';
import { CheckCircle, ArrowRight, ShoppingBag, Home } from 'lucide-react';
import { Link, useParams, useLocation } from 'react-router-dom';

const OrderSuccess = () => {
    const { id } = useParams<{ id: string }>();
    const location = useLocation();
    const orderData = location.state?.order;

    return (
        <div className="min-h-screen bg-white flex items-center justify-center p-6">
            <motion.div
                initial={{ opacity: 0, y: 30 }}
                animate={{ opacity: 1, y: 0 }}
                className="max-w-xl w-full text-center space-y-12"
            >
                <div className="relative inline-block">
                    <motion.div
                        initial={{ scale: 0 }}
                        animate={{ scale: 1 }}
                        transition={{ type: 'spring', damping: 12, delay: 0.2 }}
                        className="w-32 h-32 bg-green-50 rounded-full flex items-center justify-center relative z-10 mx-auto"
                    >
                        <CheckCircle className="w-16 h-16 text-green-500" />
                    </motion.div>
                    <motion.div
                        initial={{ opacity: 0, scale: 0.5 }}
                        animate={{ opacity: 1, scale: 1.5 }}
                        transition={{ duration: 1, repeat: Infinity, repeatType: 'reverse' }}
                        className="absolute inset-0 bg-green-100 rounded-full z-0 blur-2xl"
                    />
                </div>

                <div className="space-y-4">
                    <span className="text-[10px] uppercase tracking-[0.4em] text-green-600 font-bold block">Đặt hàng số {id?.substring(0, 8)}</span>
                    <h1 className="text-5xl font-display text-coffee-800">Cảm ơn bạn!</h1>
                    <p className="text-coffee-400 font-light leading-relaxed max-w-sm mx-auto">
                        Đơn hàng của bạn đã được tiếp nhận và đang được chuẩn bị bởi những chuyên gia pha chế tâm huyết nhất.
                    </p>
                </div>

                <div className="bg-coffee-50 p-8 space-y-4 text-left">
                    <div className="flex justify-between items-center text-xs">
                        <span className="text-coffee-400 uppercase tracking-widest font-bold">Mã đơn hàng</span>
                        <span className="text-coffee-800 font-bold tabular-nums">#{id?.substring(0, 8).toUpperCase()}</span>
                    </div>
                    {orderData?.totalAmount && (
                        <div className="flex justify-between items-center text-xs">
                            <span className="text-coffee-400 uppercase tracking-widest font-bold">Tổng thanh toán</span>
                            <span className="text-coffee-800 font-bold">
                                {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(orderData.totalAmount)}
                            </span>
                        </div>
                    )}
                    <div className="flex justify-between items-center text-xs">
                        <span className="text-coffee-400 uppercase tracking-widest font-bold">Trạng thái</span>
                        <span className="px-3 py-1 bg-blue-100 text-blue-600 rounded-full text-[8px] font-bold uppercase tracking-widest">Đang chuẩn bị</span>
                    </div>
                </div>

                <div className="flex flex-col sm:flex-row gap-6">
                    <Link to="/products" className="flex-1 bg-coffee-800 text-white h-16 uppercase tracking-[0.2em] text-[10px] font-bold hover:bg-coffee-900 transition-all flex items-center justify-center gap-4 group">
                        <ShoppingBag className="w-4 h-4" />
                        Tiếp tục mua sắm
                        <ArrowRight className="w-4 h-4 transition-transform group-hover:translate-x-1" />
                    </Link>
                    <Link to="/" className="flex-1 border border-coffee-100 text-coffee-800 h-16 uppercase tracking-[0.2em] text-[10px] font-bold hover:bg-coffee-50 transition-all flex items-center justify-center gap-4">
                        <Home className="w-4 h-4" />
                        Về trang chủ
                    </Link>
                </div>

                <p className="text-[10px] uppercase tracking-widest text-coffee-300">
                    Một email xác nhận đã được gửi đến bạn. <br /> Mọi thắc mắc xin vui lòng liên hệ 1900 xxxx.
                </p>
            </motion.div>
        </div>
    );
};

export default OrderSuccess;
