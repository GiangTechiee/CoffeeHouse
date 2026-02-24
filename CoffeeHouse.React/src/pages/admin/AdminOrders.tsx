import { useEffect, useState } from 'react';
import {
    Search,
    Filter,
    MoreHorizontal,
    Eye,
    Clock,
    CheckCircle2,
    XCircle,
    Truck,
    Calendar,
    ChevronLeft,
    ChevronRight,
    ShoppingBag
} from 'lucide-react';
import api from '../../services/api';
import type { ApiResponse, PaginatedResponse } from '../../types';

const AdminOrders = () => {
    const [orders, setOrders] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [searchDate, setSearchDate] = useState('');

    useEffect(() => {
        const fetchOrders = async () => {
            setLoading(true);
            try {
                const response = await api.get<ApiResponse<PaginatedResponse<any>>>('/v1/admin/sales-orders', {
                    params: {
                        page,
                        searchDate: searchDate || undefined
                    }
                });
                if (response.data.success) {
                    setOrders(response.data.data.items);
                    setTotalPages(response.data.data.totalPages);
                }
            } catch (error) {
                console.error('Failed to fetch orders:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchOrders();
    }, [page, searchDate]);

    const getStatusStyle = (status: string) => {
        switch (status?.toLowerCase()) {
            case 'completed':
            case 'delivered':
                return { bg: 'bg-emerald-50 text-emerald-600 border-emerald-100', icon: CheckCircle2 };
            case 'pending':
            case 'processing':
                return { bg: 'bg-amber-50 text-amber-600 border-amber-100', icon: Clock };
            case 'cancelled':
                return { bg: 'bg-rose-50 text-rose-600 border-rose-100', icon: XCircle };
            case 'shipping':
                return { bg: 'bg-blue-50 text-blue-600 border-blue-100', icon: Truck };
            default:
                return { bg: 'bg-slate-50 text-slate-600 border-slate-100', icon: Clock };
        }
    };

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
                <div>
                    <h1 className="text-3xl font-display text-slate-800">Quản lý đơn hàng</h1>
                    <p className="text-sm text-slate-500 mt-1">Theo dõi và cập nhật trạng thái đơn hàng của khách.</p>
                </div>
                <button className="flex items-center gap-2 px-6 py-3 bg-white border border-slate-200 text-slate-700 rounded-xl text-sm font-bold hover:bg-slate-50 transition-all">
                    <Calendar className="w-5 h-5" />
                    Xuất báo cáo (CSV)
                </button>
            </div>

            {/* Toolbar */}
            <div className="flex flex-col md:flex-row gap-4">
                <div className="flex-1 relative group">
                    <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
                    <input
                        type="text"
                        placeholder="Tìm theo mã đơn hàng, tên khách hàng..."
                        className="w-full pl-11 pr-4 py-3 bg-white border border-slate-200 rounded-xl text-sm outline-none focus:ring-2 focus:ring-coffee-100 transition-all"
                    />
                </div>
                <div className="flex gap-4">
                    <input
                        type="date"
                        value={searchDate}
                        onChange={(e) => setSearchDate(e.target.value)}
                        className="px-4 py-3 bg-white border border-slate-200 rounded-xl text-sm outline-none focus:ring-2 focus:ring-coffee-100 transition-all cursor-pointer"
                    />
                    <button className="px-4 py-3 bg-white border border-slate-200 rounded-xl text-sm font-bold text-slate-600 hover:bg-slate-50 transition-all flex items-center gap-2">
                        <Filter className="w-4 h-4" />
                        Bộ lọc
                    </button>
                </div>
            </div>

            {/* Orders Table */}
            <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full text-left">
                        <thead>
                            <tr className="bg-slate-50/50 border-b border-slate-100">
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Đơn hàng</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Khách hàng</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Ngày đặt</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Tổng tiền</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Trạng thái</th>
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400 text-right">Chi tiết</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-50 text-sm">
                            {loading ? (
                                Array.from({ length: 5 }).map((_, i) => (
                                    <tr key={i} className="animate-pulse">
                                        <td colSpan={6} className="px-8 py-5">
                                            <div className="h-12 bg-slate-50 rounded-xl w-full"></div>
                                        </td>
                                    </tr>
                                ))
                            ) : orders.length === 0 ? (
                                <tr>
                                    <td colSpan={6} className="px-8 py-20 text-center text-slate-400 font-light italic">
                                        Chưa có đơn hàng nào được ghi nhận.
                                    </td>
                                </tr>
                            ) : (
                                orders.map((order) => {
                                    const statusStyle = getStatusStyle(order.status);
                                    const StatusIcon = statusStyle.icon;
                                    return (
                                        <tr key={order.orderId} className="hover:bg-slate-50/50 transition-colors group">
                                            <td className="px-8 py-5">
                                                <div className="flex items-center gap-3">
                                                    <div className="w-10 h-10 bg-slate-100 rounded-lg flex items-center justify-center text-slate-400">
                                                        <ShoppingBag className="w-5 h-5" />
                                                    </div>
                                                    <div>
                                                        <p className="font-bold text-slate-800">#{order.orderId.substring(0, 8).toUpperCase()}</p>
                                                        <p className="text-[10px] text-slate-400 font-medium">Bởi: {order.employeeName || 'Hệ thống'}</p>
                                                    </div>
                                                </div>
                                            </td>
                                            <td className="px-6 py-5">
                                                <p className="font-bold text-slate-800">{order.customerName}</p>
                                                <p className="text-[10px] text-slate-400">{order.customerEmail || 'Guest'}</p>
                                            </td>
                                            <td className="px-6 py-5 text-slate-500 font-medium">
                                                {new Date(order.orderDate).toLocaleDateString('vi-VN', {
                                                    day: '2-digit', month: '2-digit', year: 'numeric',
                                                    hour: '2-digit', minute: '2-digit'
                                                })}
                                            </td>
                                            <td className="px-6 py-5 font-bold text-slate-800">
                                                {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(order.totalAmount)}
                                            </td>
                                            <td className="px-6 py-5">
                                                <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full border ${statusStyle.bg}`}>
                                                    <StatusIcon className="w-3 h-3" />
                                                    <span className="text-[10px] font-bold uppercase tracking-widest">{order.status}</span>
                                                </div>
                                            </td>
                                            <td className="px-8 py-5 text-right">
                                                <button className="p-2 text-slate-400 hover:text-coffee-700 hover:bg-coffee-50 rounded-lg transition-all">
                                                    <Eye className="w-4 h-4" />
                                                </button>
                                                <button className="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-lg transition-all ml-1">
                                                    <MoreHorizontal className="w-4 h-4" />
                                                </button>
                                            </td>
                                        </tr>
                                    );
                                })
                            )}
                        </tbody>
                    </table>
                </div>

                {/* Pagination */}
                <div className="px-8 py-5 border-t border-slate-50 flex items-center justify-between bg-slate-50/30">
                    <p className="text-xs text-slate-400 font-medium">Trang {page} trên {totalPages}</p>
                    <div className="flex gap-2">
                        <button
                            disabled={page === 1}
                            onClick={() => setPage(p => p - 1)}
                            className="p-2 border border-slate-200 rounded-lg disabled:opacity-30 hover:bg-white transition-all"
                        >
                            <ChevronLeft className="w-4 h-4" />
                        </button>
                        <button
                            disabled={page === totalPages}
                            onClick={() => setPage(p => p + 1)}
                            className="p-2 border border-slate-200 rounded-lg disabled:opacity-30 hover:bg-white transition-all"
                        >
                            <ChevronRight className="w-4 h-4" />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AdminOrders;
