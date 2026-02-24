import { useEffect, useState } from 'react';
import {
    Plus,
    Search,
    MoreHorizontal,
    Edit,
    Trash2,
    ChevronLeft,
    ChevronRight,
    Image as ImageIcon
} from 'lucide-react';
import api from '../../services/api';
import type { ApiResponse, PaginatedResponse, Product, Category } from '../../types';

const AdminProducts = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [selectedCategory, setSelectedCategory] = useState<number | undefined>(undefined);
    const [isAddingProduct, setIsAddingProduct] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const [prodRes, catRes] = await Promise.all([
                    api.get<ApiResponse<PaginatedResponse<Product>>>('/v1/admin/products', {
                        params: {
                            page,
                            search: search || undefined,
                            categoryId: selectedCategory || undefined
                        }
                    }),
                    api.get<ApiResponse<PaginatedResponse<Category>>>('/v1/categories')
                ]);

                if (prodRes.data.success) {
                    setProducts(prodRes.data.data.items);
                    setTotalPages(prodRes.data.data.totalPages);
                }
                if (catRes.data.success) {
                    setCategories(catRes.data.data.items);
                }
            } catch (error) {
                console.error('Failed to fetch data:', error);
            } finally {
                setLoading(false);
            }
        };

        const timeoutId = setTimeout(fetchData, 500);
        return () => clearTimeout(timeoutId);
    }, [page, search, selectedCategory]);

    const handleDelete = async (id: number) => {
        if (!window.confirm('Bạn có chắc chắn muốn xóa sản phẩm này?')) return;

        try {
            await api.delete(`/v1/admin/products/${id}`);
            setProducts(products.filter(p => p.id !== id));
        } catch (error) {
            console.error('Delete failed:', error);
            alert('Không thể xóa sản phẩm. Vui lòng thử lại.');
        }
    };

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
                <div>
                    <h1 className="text-3xl font-display text-slate-800">Quản lý sản phẩm</h1>
                    <p className="text-sm text-slate-500 mt-1">Tổng cộng {products.length} sản phẩm được tìm thấy.</p>
                </div>
                <button
                    onClick={() => setIsAddingProduct(true)}
                    className="flex items-center gap-2 px-6 py-3 bg-coffee-800 text-white rounded-xl text-sm font-bold hover:bg-coffee-900 transition-all shadow-lg shadow-coffee-900/20"
                >
                    <Plus className="w-5 h-5" />
                    Thêm sản phẩm mới
                </button>
            </div>

            {/* Filters */}
            <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-4">
                <div className="md:col-span-2 relative group">
                    <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400 group-focus-within:text-coffee-600 transition-colors" />
                    <input
                        type="text"
                        placeholder="Tìm theo tên sản phẩm..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        className="w-full pl-11 pr-4 py-3 bg-white border border-slate-200 rounded-xl text-sm focus:ring-2 focus:ring-coffee-100 focus:border-coffee-300 transition-all outline-none"
                    />
                </div>
                <select
                    value={selectedCategory || ''}
                    onChange={(e) => setSelectedCategory(e.target.value ? Number(e.target.value) : undefined)}
                    className="px-4 py-3 bg-white border border-slate-200 rounded-xl text-sm focus:ring-2 focus:ring-coffee-100 focus:border-coffee-300 outline-none transition-all appearance-none cursor-pointer"
                >
                    <option value="">Tất cả danh mục</option>
                    {categories.map(cat => (
                        <option key={cat.id} value={cat.id}>{cat.name}</option>
                    ))}
                </select>
            </div>

            {/* Products Table */}
            <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full text-left">
                        <thead>
                            <tr className="bg-slate-50/50 border-b border-slate-100">
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Sản phẩm</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Danh mục</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Giá bán</th>
                                <th className="px-6 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400">Trạng thái</th>
                                <th className="px-8 py-5 text-[10px] uppercase tracking-widest font-bold text-slate-400 text-right">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-50 text-sm">
                            {loading ? (
                                Array.from({ length: 5 }).map((_, i) => (
                                    <tr key={i} className="animate-pulse">
                                        <td className="px-8 py-5">
                                            <div className="flex items-center gap-4">
                                                <div className="w-12 h-12 bg-slate-100 rounded-lg"></div>
                                                <div className="h-4 bg-slate-100 rounded w-32"></div>
                                            </div>
                                        </td>
                                        <td colSpan={4} className="px-6 py-5">
                                            <div className="h-4 bg-slate-100 rounded w-full"></div>
                                        </td>
                                    </tr>
                                ))
                            ) : products.length === 0 ? (
                                <tr>
                                    <td colSpan={5} className="px-8 py-20 text-center text-slate-400 font-light italic">
                                        Không tìm thấy sản phẩm nào.
                                    </td>
                                </tr>
                            ) : (
                                products.map((product) => (
                                    <tr key={product.id} className="hover:bg-slate-50/50 transition-colors group">
                                        <td className="px-8 py-5">
                                            <div className="flex items-center gap-4">
                                                <div className="w-12 h-12 rounded-xl overflow-hidden bg-slate-100 shrink-0">
                                                    {product.imageUrl || product.image ? (
                                                        <img src={product.imageUrl || product.image} alt={product.name} className="w-full h-full object-cover" />
                                                    ) : (
                                                        <div className="w-full h-full flex items-center justify-center text-slate-300">
                                                            <ImageIcon className="w-5 h-5" />
                                                        </div>
                                                    )}
                                                </div>
                                                <div className="min-w-0">
                                                    <p className="font-bold text-slate-800 truncate group-hover:text-coffee-700 transition-colors">{product.name}</p>
                                                    <p className="text-[10px] text-slate-400 uppercase tracking-widest mt-0.5">ID: {product.id}</p>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5">
                                            <span className="px-3 py-1 bg-slate-100 text-slate-600 rounded-full text-[10px] font-bold uppercase tracking-widest">
                                                {product.categoryName}
                                            </span>
                                        </td>
                                        <td className="px-6 py-5 font-bold text-slate-800">
                                            {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(product.price)}
                                        </td>
                                        <td className="px-6 py-5">
                                            <div className="flex items-center gap-2">
                                                <span className="w-2 h-2 bg-emerald-500 rounded-full shadow-[0_0_8px_rgba(16,185,129,0.5)] animate-pulse"></span>
                                                <span className="text-xs font-medium text-slate-600">Đang bán</span>
                                            </div>
                                        </td>
                                        <td className="px-8 py-5">
                                            <div className="flex items-center justify-end gap-2">
                                                <button className="p-2 text-slate-400 hover:text-coffee-700 hover:bg-coffee-50 rounded-lg transition-all" title="Chỉnh sửa">
                                                    <Edit className="w-4 h-4" />
                                                </button>
                                                <button
                                                    onClick={() => handleDelete(product.id)}
                                                    className="p-2 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all"
                                                    title="Xóa"
                                                >
                                                    <Trash2 className="w-4 h-4" />
                                                </button>
                                                <button className="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-lg transition-all">
                                                    <MoreHorizontal className="w-4 h-4" />
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))
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

export default AdminProducts;
