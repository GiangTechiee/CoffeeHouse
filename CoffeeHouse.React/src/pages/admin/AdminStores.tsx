import { useEffect, useState } from 'react';
import {
    Plus,
    Search,
    Edit,
    Trash2,
    ChevronLeft,
    ChevronRight,
    Store,
    MapPin,
    Phone
} from 'lucide-react';
import api from '../../services/api';
import type { ApiResponse, PaginatedResponse, CafeStore } from '../../types';



const AdminStores = () => {
    const [stores, setStores] = useState<CafeStore[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [isAdding, setIsAdding] = useState(false);

    // Modal state for Add/Edit
    const [editingStore, setEditingStore] = useState<CafeStore | null>(null);
    const [formData, setFormData] = useState({
        storeName: '',
        address: '',
        phoneNumber: '',
        openingTime: '07:00',
        closingTime: '22:00',
        status: 'Active'
    });

    useEffect(() => {
        const fetchStores = async () => {
            setLoading(true);
            try {
                const response = await api.get<ApiResponse<PaginatedResponse<CafeStore>>>('/v1/admin/cafe-stores', {
                    params: {
                        pageNumber: page,
                        pageSize: 10,
                        searchTerm: search || undefined
                    }
                });

                if (response.data.success) {
                    setStores(response.data.data.items);
                    setTotalPages(response.data.data.totalPages);
                }
            } catch (error) {
                console.error('Failed to fetch stores:', error);
            } finally {
                setLoading(false);
            }
        };

        const timeoutId = setTimeout(fetchStores, 500);
        return () => clearTimeout(timeoutId);
    }, [page, search]);

    const handleDelete = async (id: number) => {
        if (!window.confirm('Bạn có chắc chắn muốn xóa cửa hàng này?')) return;

        try {
            await api.delete(`/v1/admin/cafe-stores/${id}`);
            // Refresh list
            setStores(stores.filter(s => s.id !== id));
        } catch (error) {
            console.error('Delete failed:', error);
            alert('Không thể xóa cửa hàng. Vui lòng thử lại.');
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (editingStore) {
                await api.put(`/v1/admin/cafe-stores/${editingStore.id}`, {
                    id: editingStore.id,
                    ...formData
                });
            } else {
                await api.post('/v1/admin/cafe-stores', formData);
            }
            // Reset and refresh
            setIsAdding(false);
            setEditingStore(null);
            setFormData({
                storeName: '',
                address: '',
                phoneNumber: '',
                openingTime: '07:00',
                closingTime: '22:00',
                status: 'Active'
            });
            window.location.reload();
        } catch (error) {
            console.error('Save failed:', error);
            alert('Lưu thất bại. Vui lòng thử lại.');
        }
    };

    const startEdit = (store: CafeStore) => {
        setEditingStore(store);
        setFormData({
            storeName: store.storeName,
            address: store.address,
            phoneNumber: store.phoneNumber,
            openingTime: store.openingTime,
            closingTime: store.closingTime,
            status: store.status
        });
        setIsAdding(true);
    };

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
                <div>
                    <h1 className="text-3xl font-display text-slate-800">Quản lý cửa hàng</h1>
                    <p className="text-sm text-slate-500 mt-1">Danh sách các chi nhánh.</p>
                </div>
                <button
                    onClick={() => {
                        setEditingStore(null);
                        setFormData({
                            storeName: '',
                            address: '',
                            phoneNumber: '',
                            openingTime: '07:00',
                            closingTime: '22:00',
                            status: 'Active'
                        });
                        setIsAdding(true);
                    }}
                    className="flex items-center gap-2 px-6 py-3 bg-coffee-800 text-white rounded-xl text-sm font-bold hover:bg-coffee-900 transition-all shadow-lg shadow-coffee-900/20"
                >
                    <Plus className="w-5 h-5" />
                    Thêm cửa hàng mới
                </button>
            </div>

            {/* Modal Form */}
            {isAdding && (
                <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
                    <div className="bg-white rounded-2xl w-full max-w-lg p-6">
                        <h2 className="text-xl font-bold mb-4">{editingStore ? 'Sửa cửa hàng' : 'Thêm cửa hàng'}</h2>
                        <form onSubmit={handleSubmit} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-slate-700 mb-1">Tên cửa hàng</label>
                                <input
                                    type="text"
                                    required
                                    value={formData.storeName}
                                    onChange={e => setFormData({ ...formData, storeName: e.target.value })}
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
                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-slate-700 mb-1">Giờ mở cửa</label>
                                    <input
                                        type="time"
                                        required
                                        value={formData.openingTime}
                                        onChange={e => setFormData({ ...formData, openingTime: e.target.value })}
                                        className="w-full px-4 py-2 border rounded-xl focus:ring-2 focus:ring-coffee-500 outline-none"
                                    />
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-slate-700 mb-1">Giờ đóng cửa</label>
                                    <input
                                        type="time"
                                        required
                                        value={formData.closingTime}
                                        onChange={e => setFormData({ ...formData, closingTime: e.target.value })}
                                        className="w-full px-4 py-2 border rounded-xl focus:ring-2 focus:ring-coffee-500 outline-none"
                                    />
                                </div>
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
                    placeholder="Tìm theo tên cửa hàng..."
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
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Cửa hàng</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Liên hệ</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Thời gian</th>
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400 text-right">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-50 text-sm">
                            {loading ? (
                                <tr><td colSpan={4} className="px-8 py-10 text-center">Đang tải...</td></tr>
                            ) : stores.length === 0 ? (
                                <tr><td colSpan={4} className="px-8 py-10 text-center text-slate-400">Không có dữ liệu</td></tr>
                            ) : (
                                stores.map((store) => (
                                    <tr key={store.id} className="hover:bg-slate-50/50 transition-colors group">
                                        <td className="px-8 py-5">
                                            <div className="flex items-center gap-3">
                                                <div className="w-10 h-10 rounded-lg bg-coffee-50 flex items-center justify-center text-coffee-600">
                                                    <Store className="w-5 h-5" />
                                                </div>
                                                <div>
                                                    <p className="font-bold text-slate-800">{store.storeName}</p>
                                                    <div className="flex items-center gap-1 text-xs text-slate-500 mt-1">
                                                        <MapPin className="w-3 h-3" />
                                                        <span className="truncate max-w-[200px]">{store.address}</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5">
                                            <div className="flex items-center gap-2 text-slate-600">
                                                <Phone className="w-3 h-3" />
                                                <span>{store.phoneNumber}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5 text-slate-600">
                                            {store.openingTime} - {store.closingTime}
                                        </td>
                                        <td className="px-8 py-5 text-right">
                                            <div className="flex items-center justify-end gap-2">
                                                <button onClick={() => startEdit(store)} className="p-2 text-slate-400 hover:text-coffee-700 hover:bg-coffee-50 rounded-lg transition-all">
                                                    <Edit className="w-4 h-4" />
                                                </button>
                                                <button onClick={() => handleDelete(store.id)} className="p-2 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all">
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

export default AdminStores;
