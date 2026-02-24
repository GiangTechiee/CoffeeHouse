import { useEffect, useState } from 'react';
import {
    Plus,
    Search,
    Edit,
    Trash2,
    ChevronLeft,
    ChevronRight,
    Tag
} from 'lucide-react';
import api from '../../services/api';
import type { ApiResponse, PaginatedResponse, Category } from '../../types';

const AdminCategories = () => {
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [isAdding, setIsAdding] = useState(false);

    // Modal state for Add/Edit
    const [editingCategory, setEditingCategory] = useState<Category | null>(null);
    const [formData, setFormData] = useState({ name: '', description: '' });

    useEffect(() => {
        const fetchCategories = async () => {
            setLoading(true);
            try {
                const response = await api.get<ApiResponse<PaginatedResponse<Category>>>('/v1/admin/product-categories', {
                    params: {
                        pageNumber: page,
                        pageSize: 10,
                        searchTerm: search || undefined
                    }
                });

                if (response.data.success) {
                    setCategories(response.data.data.items);
                    setTotalPages(response.data.data.totalPages);
                }
            } catch (error) {
                console.error('Failed to fetch categories:', error);
            } finally {
                setLoading(false);
            }
        };

        const timeoutId = setTimeout(fetchCategories, 500);
        return () => clearTimeout(timeoutId);
    }, [page, search]);

    const handleDelete = async (id: number) => {
        if (!window.confirm('Bạn có chắc chắn muốn xóa danh mục này?')) return;

        try {
            await api.delete(`/v1/admin/product-categories/${id}`);
            // Refresh list
            setCategories(categories.filter(c => c.id !== id));
        } catch (error) {
            console.error('Delete failed:', error);
            alert('Không thể xóa danh mục. Vui lòng thử lại.');
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (editingCategory) {
                await api.put(`/v1/admin/product-categories/${editingCategory.id}`, {
                    id: editingCategory.id,
                    ...formData
                });
            } else {
                await api.post('/v1/admin/product-categories', formData);
            }
            // Reset and refresh
            setIsAdding(false);
            setEditingCategory(null);
            setFormData({ name: '', description: '' });
            // Should properly refresh data here, for simplicity trigger reload via state or simple reload
            window.location.reload();
        } catch (error) {
            console.error('Save failed:', error);
            alert('Lưu thất bại. Vui lòng thử lại.');
        }
    };

    const startEdit = (category: Category) => {
        setEditingCategory(category);
        setFormData({ name: category.name, description: category.description || '' });
        setIsAdding(true);
    };

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
                <div>
                    <h1 className="text-3xl font-display text-slate-800">Quản lý danh mục</h1>
                    <p className="text-sm text-slate-500 mt-1">Danh sách các loại sản phẩm.</p>
                </div>
                <button
                    onClick={() => {
                        setEditingCategory(null);
                        setFormData({ name: '', description: '' });
                        setIsAdding(true);
                    }}
                    className="flex items-center gap-2 px-6 py-3 bg-coffee-800 text-white rounded-xl text-sm font-bold hover:bg-coffee-900 transition-all shadow-lg shadow-coffee-900/20"
                >
                    <Plus className="w-5 h-5" />
                    Thêm danh mục mới
                </button>
            </div>

            {/* Modal Form */}
            {isAdding && (
                <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
                    <div className="bg-white rounded-2xl w-full max-w-md p-6">
                        <h2 className="text-xl font-bold mb-4">{editingCategory ? 'Sửa danh mục' : 'Thêm danh mục'}</h2>
                        <form onSubmit={handleSubmit} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-slate-700 mb-1">Tên danh mục</label>
                                <input
                                    type="text"
                                    required
                                    value={formData.name}
                                    onChange={e => setFormData({ ...formData, name: e.target.value })}
                                    className="w-full px-4 py-2 border rounded-xl focus:ring-2 focus:ring-coffee-500 outline-none"
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-slate-700 mb-1">Mô tả</label>
                                <textarea
                                    value={formData.description}
                                    onChange={e => setFormData({ ...formData, description: e.target.value })}
                                    className="w-full px-4 py-2 border rounded-xl focus:ring-2 focus:ring-coffee-500 outline-none"
                                    rows={3}
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
                    placeholder="Tìm theo tên danh mục..."
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
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Tên</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Mô tả</th>
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400 text-right">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-50 text-sm">
                            {loading ? (
                                <tr><td colSpan={3} className="px-8 py-10 text-center">Đang tải...</td></tr>
                            ) : categories.length === 0 ? (
                                <tr><td colSpan={3} className="px-8 py-10 text-center text-slate-400">Không có dữ liệu</td></tr>
                            ) : (
                                categories.map((cat) => (
                                    <tr key={cat.id} className="hover:bg-slate-50/50 transition-colors group">
                                        <td className="px-8 py-5">
                                            <div className="flex items-center gap-3">
                                                <div className="w-10 h-10 rounded-lg bg-coffee-50 flex items-center justify-center text-coffee-600">
                                                    <Tag className="w-5 h-5" />
                                                </div>
                                                <span className="font-bold text-slate-800">{cat.name}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5 text-slate-600 max-w-md truncate">
                                            {cat.description}
                                        </td>
                                        <td className="px-8 py-5 text-right">
                                            <div className="flex items-center justify-end gap-2">
                                                <button onClick={() => startEdit(cat)} className="p-2 text-slate-400 hover:text-coffee-700 hover:bg-coffee-50 rounded-lg transition-all">
                                                    <Edit className="w-4 h-4" />
                                                </button>
                                                <button onClick={() => handleDelete(cat.id)} className="p-2 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all">
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

export default AdminCategories;
