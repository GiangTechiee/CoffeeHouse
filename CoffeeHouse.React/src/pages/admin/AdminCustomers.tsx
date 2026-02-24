import { useEffect, useState } from 'react';
import {
    Plus,
    Search,
    Edit,
    Trash2,
    ChevronLeft,
    ChevronRight,
    Users,
    MapPin,
    Phone
} from 'lucide-react';
import api from '../../services/api';
import type { ApiResponse, PaginatedResponse, Customer } from '../../types';

const AdminCustomers = () => {
    const [customers, setCustomers] = useState<Customer[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [isAdding, setIsAdding] = useState(false);

    // Modal state for Add/Edit
    const [editingCustomer, setEditingCustomer] = useState<Customer | null>(null);
    const [formData, setFormData] = useState({
        name: '',
        phoneNumber: '',
        address: ''
    });

    useEffect(() => {
        const fetchCustomers = async () => {
            setLoading(true);
            try {
                const response = await api.get<ApiResponse<PaginatedResponse<Customer>>>('/v1/admin/customers', {
                    params: {
                        page,
                        pageSize: 10,
                        search: search || undefined
                    }
                });

                if (response.data.success) {
                    setCustomers(response.data.data.items);
                    setTotalPages(response.data.data.totalPages);
                }
            } catch (error) {
                console.error('Failed to fetch customers:', error);
            } finally {
                setLoading(false);
            }
        };

        const timeoutId = setTimeout(fetchCustomers, 500);
        return () => clearTimeout(timeoutId);
    }, [page, search]);

    const handleDelete = async (id: number) => {
        if (!window.confirm('Bạn có chắc chắn muốn xóa khách hàng này?')) return;

        try {
            await api.delete(`/v1/admin/customers/${id}`);
            // Refresh list
            setCustomers(customers.filter(c => c.customerId !== id));
        } catch (error) {
            console.error('Delete failed:', error);
            alert('Không thể xóa khách hàng. Vui lòng thử lại.');
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (editingCustomer) {
                await api.put(`/v1/admin/customers/${editingCustomer.customerId}`, {
                    id: editingCustomer.customerId,
                    ...formData
                });
            } else {
                await api.post('/v1/admin/customers', formData);
            }
            // Reset and refresh
            setIsAdding(false);
            setEditingCustomer(null);
            setFormData({
                name: '',
                phoneNumber: '',
                address: ''
            });
            window.location.reload();
        } catch (error) {
            console.error('Save failed:', error);
            alert('Lưu thất bại. Vui lòng thử lại.');
        }
    };

    const startEdit = (customer: Customer) => {
        setEditingCustomer(customer);
        setFormData({
            name: customer.customerName,
            phoneNumber: customer.phoneNumber,
            address: customer.address
        });
        setIsAdding(true);
    };

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
                <div>
                    <h1 className="text-3xl font-display text-slate-800">Quản lý khách hàng</h1>
                    <p className="text-sm text-slate-500 mt-1">Danh sách khách hàng thân thiết.</p>
                </div>
                <button
                    onClick={() => {
                        setEditingCustomer(null);
                        setFormData({
                            name: '',
                            phoneNumber: '',
                            address: ''
                        });
                        setIsAdding(true);
                    }}
                    className="flex items-center gap-2 px-6 py-3 bg-coffee-800 text-white rounded-xl text-sm font-bold hover:bg-coffee-900 transition-all shadow-lg shadow-coffee-900/20"
                >
                    <Plus className="w-5 h-5" />
                    Thêm khách hàng mới
                </button>
            </div>

            {/* Modal Form */}
            {isAdding && (
                <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
                    <div className="bg-white rounded-2xl w-full max-w-lg p-6">
                        <h2 className="text-xl font-bold mb-4">{editingCustomer ? 'Sửa khách hàng' : 'Thêm khách hàng'}</h2>
                        <form onSubmit={handleSubmit} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-slate-700 mb-1">Tên khách hàng</label>
                                <input
                                    type="text"
                                    required
                                    value={formData.name}
                                    onChange={e => setFormData({ ...formData, name: e.target.value })}
                                    className="w-full px-4 py-2 border rounded-xl focus:ring-2 focus:ring-coffee-500 outline-none"
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-slate-700 mb-1">Số điện thoại</label>
                                <input
                                    type="text"
                                    required
                                    value={formData.phoneNumber}
                                    onChange={e => setFormData({ ...formData, phoneNumber: e.target.value })}
                                    className="w-full px-4 py-2 border rounded-xl focus:ring-2 focus:ring-coffee-500 outline-none"
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-slate-700 mb-1">Địa chỉ</label>
                                <input
                                    type="text"
                                    required
                                    value={formData.address}
                                    onChange={e => setFormData({ ...formData, address: e.target.value })}
                                    className="w-full px-4 py-2 border rounded-xl focus:ring-2 focus:ring-coffee-500 outline-none"
                                />
                            </div>
                            <div className="flex justify-end gap-3 mt-6">
                                <button
                                    type="button"
                                    onClick={() => setIsAdding(false)}
                                    className="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-lg"
                                >
                                    Hủy
                                </button>
                                <button
                                    type="submit"
                                    className="px-4 py-2 bg-coffee-800 text-white rounded-lg hover:bg-coffee-900"
                                >
                                    Lưu
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Filters */}
            <div className="relative group max-w-md">
                <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400 group-focus-within:text-coffee-600 transition-colors" />
                <input
                    type="text"
                    placeholder="Tìm theo tên khách hàng..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="w-full pl-11 pr-4 py-3 bg-white border border-slate-200 rounded-xl text-sm focus:ring-2 focus:ring-coffee-100 focus:border-coffee-300 transition-all outline-none"
                />
            </div>

            {/* Table */}
            <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full text-left">
                        <thead>
                            <tr className="bg-slate-50/50 border-b border-slate-100">
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Khách hàng</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Liên hệ</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Địa chỉ</th>
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400 text-right">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-50 text-sm">
                            {loading ? (
                                <tr><td colSpan={4} className="px-8 py-10 text-center">Đang tải...</td></tr>
                            ) : customers.length === 0 ? (
                                <tr><td colSpan={4} className="px-8 py-10 text-center text-slate-400">Không có dữ liệu</td></tr>
                            ) : (
                                customers.map((customer) => (
                                    <tr key={customer.customerId} className="hover:bg-slate-50/50 transition-colors group">
                                        <td className="px-8 py-5">
                                            <div className="flex items-center gap-3">
                                                <div className="w-10 h-10 rounded-lg bg-coffee-50 flex items-center justify-center text-coffee-600">
                                                    <Users className="w-5 h-5" />
                                                </div>
                                                <span className="font-bold text-slate-800">{customer.customerName}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5">
                                            <div className="flex items-center gap-2 text-slate-600">
                                                <Phone className="w-3 h-3" />
                                                <span>{customer.phoneNumber}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5 text-slate-600">
                                            <div className="flex items-center gap-2 text-slate-600">
                                                <MapPin className="w-3 h-3" />
                                                <span className="truncate max-w-[200px]">{customer.address}</span>
                                            </div>
                                        </td>
                                        <td className="px-8 py-5 text-right">
                                            <div className="flex items-center justify-end gap-2">
                                                <button onClick={() => startEdit(customer)} className="p-2 text-slate-400 hover:text-coffee-700 hover:bg-coffee-50 rounded-lg transition-all">
                                                    <Edit className="w-4 h-4" />
                                                </button>
                                                <button onClick={() => handleDelete(customer.customerId)} className="p-2 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all">
                                                    <Trash2 className="w-4 h-4" />
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </div>
                {/* Pagination - Simplified */}
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

export default AdminCustomers;
