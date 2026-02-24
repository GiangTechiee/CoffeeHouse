import { useEffect, useState } from 'react';
import {
    TrendingUp,
    Users,
    ShoppingBag,
    DollarSign,
    ArrowUpRight,
    ArrowDownRight,
    Calendar,
    Filter
} from 'lucide-react';
import api from '../../services/api';
import type { ApiResponse } from '../../types';
import { motion } from 'framer-motion';

const Dashboard = () => {
    const [stats, setStats] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchStats = async () => {
            try {
                const response = await api.get<ApiResponse<any>>('/v1/admin/reports/dashboard-stats');
                if (response.data.success) {
                    setStats(response.data.data);
                }
            } catch (error) {
                console.error('Failed to fetch stats:', error);

                // Set default zero values if API fails, avoiding mock data to prevent confusion
                setStats({
                    totalRevenue: 0,
                    totalOrders: 0,
                    totalCustomers: 0,
                    revenueGrowth: 0,
                    orderGrowth: 0,
                    customerGrowth: 0,
                    conversionRate: 0,
                    conversionGrowth: 0
                });
            } finally {
                setLoading(false);
            }
        };
        fetchStats();
    }, []);

    const statCards = [
        {
            label: 'Doanh thu',
            value: stats?.totalRevenue || 0,
            icon: DollarSign,
            color: 'bg-emerald-500',
            growth: stats?.revenueGrowth || 0,
            isPrice: true
        },
        {
            label: 'Đơn hàng',
            value: stats?.totalOrders || 0,
            icon: ShoppingBag,
            color: 'bg-blue-500',
            growth: stats?.orderGrowth || 0,
            isPrice: false
        },
        {
            label: 'Khách hàng',
            value: stats?.totalCustomers || 0,
            icon: Users,
            color: 'bg-violet-500',
            growth: stats?.customerGrowth || 0,
            isPrice: false
        },
        {
            label: 'Tỷ lệ chuyển đổi',
            value: '64.2%',
            icon: TrendingUp,
            color: 'bg-amber-500',
            growth: 1.2,
            isPrice: false
        },
    ];

    return (
        <div className="space-y-10">
            {/* Page Header */}
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
                <div>
                    <h1 className="text-3xl font-display text-slate-800">Báo cáo tổng quan</h1>
                    <p className="text-sm text-slate-500 mt-1">Chào mừng trở lại, đây là những gì đang diễn ra hôm nay.</p>
                </div>
                <div className="flex items-center gap-3">
                    <button className="flex items-center gap-2 px-4 py-2.5 bg-white border border-slate-200 rounded-xl text-sm font-medium text-slate-600 hover:bg-slate-50 transition-all">
                        <Calendar className="w-4 h-4" />
                        7 ngày qua
                    </button>
                    <button className="flex items-center gap-2 px-4 py-2.5 bg-coffee-800 text-white rounded-xl text-sm font-medium hover:bg-coffee-900 transition-all shadow-lg shadow-coffee-900/20">
                        <Filter className="w-4 h-4" />
                        Lọc dữ liệu
                    </button>
                </div>
            </div>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                {statCards.map((card, idx) => (
                    <motion.div
                        key={card.label}
                        initial={{ opacity: 0, y: 20 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: idx * 0.1 }}
                        className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm"
                    >
                        <div className="flex justify-between items-start mb-4">
                            <div className={`p-3 rounded-xl ${card.color} text-white`}>
                                <card.icon className="w-6 h-6" />
                            </div>
                            <div className={`flex items-center gap-1 text-xs font-bold ${card.growth >= 0 ? 'text-emerald-600' : 'text-red-500'
                                }`}>
                                {card.growth >= 0 ? <ArrowUpRight className="w-3 h-3" /> : <ArrowDownRight className="w-3 h-3" />}
                                {Math.abs(card.growth)}%
                            </div>
                        </div>
                        <div className="space-y-1">
                            <p className="text-sm text-slate-500 font-medium">{card.label}</p>
                            <h3 className="text-2xl font-bold text-slate-800">
                                {card.isPrice
                                    ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(card.value)
                                    : card.value}
                            </h3>
                        </div>
                    </motion.div>
                ))}
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Sales Chart Placeholder */}
                <div className="lg:col-span-2 bg-white p-8 rounded-3xl border border-slate-200 shadow-sm min-h-[400px]">
                    <div className="flex justify-between items-center mb-10">
                        <h3 className="text-xl font-bold text-slate-800">Phân tích doanh thu</h3>
                        <div className="flex gap-2">
                            <button className="px-3 py-1.5 text-xs font-bold bg-slate-100 text-slate-600 rounded-lg">Tháng</button>
                            <button className="px-3 py-1.5 text-xs font-bold text-slate-400">Tuần</button>
                        </div>
                    </div>
                    <div className="h-64 flex items-end gap-4 px-4 overflow-hidden">
                        {[40, 60, 45, 90, 65, 80, 55, 70, 95, 50, 85, 75].map((val, i) => (
                            <motion.div
                                key={i}
                                initial={{ height: 0 }}
                                animate={{ height: `${val}%` }}
                                transition={{ delay: 0.5 + i * 0.05, duration: 1 }}
                                className="flex-1 bg-coffee-100/50 rounded-t-lg relative group"
                            >
                                <div className="absolute inset-x-0 bottom-0 bg-coffee-700 rounded-t-lg transition-all duration-500 group-hover:bg-coffee-900 group-hover:h-full opacity-0 group-hover:opacity-100" style={{ height: '30%' }} />
                            </motion.div>
                        ))}
                    </div>
                    <div className="flex justify-between mt-6 px-4 text-[10px] uppercase font-bold text-slate-400 tracking-widest">
                        <span>Jan</span>
                        <span>Mar</span>
                        <span>May</span>
                        <span>Jul</span>
                        <span>Sep</span>
                        <span>Nov</span>
                    </div>
                </div>

                {/* Top Products */}
                <div className="bg-white p-8 rounded-3xl border border-slate-200 shadow-sm">
                    <h3 className="text-xl font-bold text-slate-800 mb-8">Sản phẩm bán chạy</h3>
                    <div className="space-y-6">
                        {[
                            { name: 'Cà phê Muối', category: 'Cà phê', sales: 450, growth: 15 },
                            { name: 'Trà Đào Cam Sả', category: 'Trà', sales: 380, growth: 8 },
                            { name: 'Bạc Xỉu', category: 'Cà phê', sales: 320, growth: -5 },
                            { name: 'Croissant Hạnh Nhân', category: 'Bánh ngọt', sales: 290, growth: 12 },
                        ].map((item, idx) => (
                            <div key={idx} className="flex items-center justify-between group cursor-pointer">
                                <div className="flex items-center gap-4">
                                    <div className="w-12 h-12 bg-slate-50 rounded-xl flex items-center justify-center text-coffee-800 font-bold group-hover:bg-coffee-50 transition-colors">
                                        {item.name.charAt(0)}
                                    </div>
                                    <div>
                                        <p className="text-sm font-bold text-slate-800 group-hover:text-coffee-600 transition-colors">{item.name}</p>
                                        <p className="text-[10px] uppercase text-slate-400 tracking-widest">{item.category}</p>
                                    </div>
                                </div>
                                <div className="text-right">
                                    <p className="text-sm font-bold text-slate-800">{item.sales}</p>
                                    <p className={`text-[10px] font-bold ${item.growth >= 0 ? 'text-emerald-500' : 'text-red-400'}`}>
                                        {item.growth >= 0 ? '+' : ''}{item.growth}%
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                    <button className="w-full mt-10 py-3 text-xs font-bold text-coffee-700 bg-coffee-50 hover:bg-coffee-100 rounded-xl transition-all">
                        Xem tất cả sản phẩm
                    </button>
                </div>
            </div>
        </div>
    );
};

export default Dashboard;
