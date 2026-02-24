import { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import {
    LayoutDashboard,
    Coffee,
    Layers,
    ShoppingBag,
    Users,
    LogOut,
    Menu,
    X,
    Bell,
    Search,
    ChevronDown,
    Store
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { motion, AnimatePresence } from 'framer-motion';

const AdminLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);
    const { user, logout } = useAuth();
    const location = useLocation();
    const navigate = useNavigate();

    const menuItems = [
        { icon: LayoutDashboard, label: 'Tổng quan', path: '/admin' },
        { icon: ShoppingBag, label: 'Đơn hàng', path: '/admin/orders' },
        { icon: Coffee, label: 'Sản phẩm', path: '/admin/products' },
        { icon: Layers, label: 'Danh mục', path: '/admin/categories' },
        { icon: Store, label: 'Cửa hàng', path: '/admin/stores' },
        { icon: Users, label: 'Khách hàng', path: '/admin/customers' },
    ];

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <div className="min-h-screen bg-slate-50 flex">
            {/* Sidebar */}
            <aside
                className={`fixed lg:static inset-y-0 left-0 z-50 w-72 bg-coffee-900 text-white transition-all duration-300 transform ${isSidebarOpen ? 'translate-x-0' : '-translate-x-full lg:w-20'
                    }`}
            >
                <div className="h-full flex flex-col">
                    {/* Logo Area */}
                    <div className="h-20 flex items-center justify-between px-6 border-b border-white/5">
                        <div className={`flex items-center gap-3 overflow-hidden transition-all duration-300 ${!isSidebarOpen && 'lg:hidden'}`}>
                            <div className="w-8 h-8 bg-white rounded-lg flex items-center justify-center">
                                <Coffee className="w-5 h-5 text-coffee-900" />
                            </div>
                            <span className="font-display text-xl tracking-tight">C.House <span className="text-[10px] bg-white/10 px-2 py-0.5 rounded ml-1 uppercase opacity-50 font-sans tracking-widest">Admin</span></span>
                        </div>
                        <button
                            onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                            className="p-2 hover:bg-white/10 rounded-lg transition-colors"
                        >
                            {isSidebarOpen ? <X className="w-5 h-5 lg:hidden" /> : <Menu className="w-5 h-5" />}
                        </button>
                    </div>

                    {/* Navigation */}
                    <nav className="flex-1 py-6 px-4 space-y-2 overflow-y-auto custom-scrollbar">
                        {menuItems.map((item) => {
                            const isActive = location.pathname === item.path;
                            return (
                                <Link
                                    key={item.path}
                                    to={item.path}
                                    className={`flex items-center gap-4 px-4 py-3 rounded-xl transition-all group ${isActive
                                        ? 'bg-white text-coffee-900 shadow-lg shadow-black/20'
                                        : 'hover:bg-white/5 text-white/60 hover:text-white'
                                        }`}
                                >
                                    <item.icon className={`w-5 h-5 shrink-0 ${isActive ? 'text-coffee-900' : 'group-hover:scale-110 transition-transform'}`} />
                                    <span className={`font-medium whitespace-nowrap overflow-hidden transition-all ${!isSidebarOpen && 'lg:hidden'}`}>
                                        {item.label}
                                    </span>
                                </Link>
                            );
                        })}
                    </nav>

                    {/* Sidebar Footer */}
                    <div className="p-4 border-t border-white/5">
                        <button
                            onClick={handleLogout}
                            className={`w-full flex items-center gap-4 px-4 py-3 rounded-xl text-red-400 hover:bg-red-500/10 transition-all ${!isSidebarOpen && 'lg:justify-center'}`}
                        >
                            <LogOut className="w-5 h-5 shrink-0" />
                            <span className={`font-medium transition-all ${!isSidebarOpen && 'lg:hidden'}`}>Đăng xuất</span>
                        </button>
                    </div>
                </div>
            </aside>

            {/* Main Content */}
            <div className="flex-1 flex flex-col min-w-0">
                {/* Top Header */}
                <header className="h-20 bg-white border-b border-slate-200 px-8 flex items-center justify-between sticky top-0 z-40">
                    <div className="flex items-center gap-4 lg:hidden">
                        <button onClick={() => setIsSidebarOpen(true)} className="p-2 hover:bg-slate-100 rounded-lg">
                            <Menu className="w-6 h-6 text-slate-600" />
                        </button>
                    </div>

                    {/* Search Bar */}
                    <div className="hidden md:flex items-center bg-slate-100 rounded-xl px-4 py-2 w-96 border border-transparent focus-within:border-coffee-200 focus-within:bg-white transition-all">
                        <Search className="w-4 h-4 text-slate-400" />
                        <input
                            type="text"
                            placeholder="Tìm kiếm báo cáo, đơn hàng..."
                            className="bg-transparent border-none focus:ring-0 text-sm w-full px-3 text-slate-600"
                        />
                    </div>

                    {/* Actions */}
                    <div className="flex items-center gap-6">
                        <button className="relative p-2 text-slate-500 hover:bg-slate-100 rounded-full transition-all">
                            <Bell className="w-5 h-5" />
                            <span className="absolute top-2 right-2 w-2 h-2 bg-red-500 rounded-full border-2 border-white"></span>
                        </button>

                        <div className="h-8 w-px bg-slate-200 mx-1"></div>

                        <div className="flex items-center gap-4 group cursor-pointer p-1 rounded-xl hover:bg-slate-100 transition-all">
                            <div className="w-10 h-10 bg-coffee-800 rounded-lg flex items-center justify-center text-white font-bold">
                                {user?.name?.charAt(0) || 'A'}
                            </div>
                            <div className="hidden sm:block">
                                <p className="text-sm font-bold text-slate-800 leading-none mb-1">{user?.name || 'Admin User'}</p>
                                <p className="text-[10px] uppercase font-bold text-slate-400 tracking-widest">{user?.email || 'admin@coffeehouse.com'}</p>
                            </div>
                            <ChevronDown className="w-4 h-4 text-slate-400" />
                        </div>
                    </div>
                </header>

                {/* Page Content */}
                <main className="flex-1 p-8 overflow-y-auto">
                    <AnimatePresence mode="wait">
                        <motion.div
                            key={location.pathname}
                            initial={{ opacity: 0, y: 10 }}
                            animate={{ opacity: 1, y: 0 }}
                            exit={{ opacity: 0, y: -10 }}
                            transition={{ duration: 0.2 }}
                        >
                            {children}
                        </motion.div>
                    </AnimatePresence>
                </main>
            </div>
        </div>
    );
};

export default AdminLayout;
