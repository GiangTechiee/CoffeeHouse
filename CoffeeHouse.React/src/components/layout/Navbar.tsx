import { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Coffee, Search, User, ShoppingCart, Menu, X, ChevronDown, LogOut, Settings, ReceiptText, LogIn } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { cn } from '../../utils/cn';
import { useAuth } from '../../context/AuthContext';
import { useCart } from '../../context/CartContext';

const Navbar = () => {
    const [isScrolled, setIsScrolled] = useState(false);
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
    const [isUserDropdownOpen, setIsUserDropdownOpen] = useState(false);
    const location = useLocation();

    const { user, logout } = useAuth();
    const { totalItems } = useCart();

    useEffect(() => {
        const handleScroll = () => {
            setIsScrolled(window.scrollY > 50);
        };
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, []);

    const navLinks = [
        { name: 'Trang chủ', path: '/' },
        { name: 'Sản phẩm', path: '/products' },
        { name: 'Tin tức', path: '/news' },
        { name: 'Giới thiệu', path: '/about' },
    ];

    return (
        <>
            <header
                className={cn(
                    "fixed top-0 left-0 right-0 z-50 transition-all duration-500 ease-in-out px-6 md:px-12",
                    isScrolled
                        ? "py-4 bg-white/80 backdrop-blur-md shadow-sm border-b border-coffee-100"
                        : "py-8 bg-transparent"
                )}
            >
                <div className="max-w-7xl mx-auto flex items-center justify-between">

                    {/* Logo */}
                    <Link
                        to="/"
                        className="flex items-center gap-2 group"
                    >
                        <div className="bg-coffee-700 p-2 rounded-none group-hover:rotate-12 transition-transform duration-500">
                            <Coffee className="w-6 h-6 text-coffee-100" />
                        </div>
                        <span className="text-2xl font-display font-bold tracking-tighter text-coffee-700">
                            COFFEE HOUSE
                        </span>
                    </Link>

                    {/* Desktop Navigation */}
                    <nav className="hidden lg:flex items-center gap-10">
                        {navLinks.map((link) => (
                            <Link
                                key={link.path}
                                to={link.path}
                                className={cn(
                                    "text-xs uppercase tracking-[0.2em] font-medium transition-colors duration-300 relative group",
                                    location.pathname === link.path ? "text-coffee-700" : "text-coffee-600 hover:text-coffee-700"
                                )}
                            >
                                {link.name}
                                <span className={cn(
                                    "absolute -bottom-1 left-0 h-[1px] bg-coffee-300 transition-all duration-500",
                                    location.pathname === link.path ? "w-full" : "w-0 group-hover:w-full"
                                )} />
                            </Link>
                        ))}
                    </nav>

                    {/* Actions */}
                    <div className="flex items-center gap-5 md:gap-8">
                        <button className="text-coffee-600 hover:text-coffee-900 transition-transform duration-300 hover:scale-110">
                            <Search className="w-5 h-5" />
                        </button>

                        {/* User Dropdown */}
                        <div className="relative">
                            {user ? (
                                <>
                                    <button
                                        onClick={() => setIsUserDropdownOpen(!isUserDropdownOpen)}
                                        className="flex items-center gap-2 text-coffee-600 hover:text-coffee-900 font-medium text-xs uppercase tracking-widest"
                                    >
                                        <User className="w-5 h-5" />
                                        <span className="hidden sm:inline">{user.name}</span>
                                        <ChevronDown className={cn("w-3 h-3 transition-transform duration-300", isUserDropdownOpen && "rotate-180")} />
                                    </button>

                                    <AnimatePresence>
                                        {isUserDropdownOpen && (
                                            <motion.div
                                                initial={{ opacity: 0, y: 10 }}
                                                animate={{ opacity: 1, y: 0 }}
                                                exit={{ opacity: 0, y: 10 }}
                                                className="absolute right-0 mt-4 w-56 bg-white shadow-2xl border border-coffee-100 p-2"
                                            >
                                                <div className="px-4 py-2 text-xs text-slate-400 border-b border-coffee-50 mb-2 truncate">
                                                    {user.email}
                                                </div>
                                                <Link to="/profile" className="flex items-center gap-3 px-4 py-3 text-xs uppercase tracking-widest text-coffee-600 hover:bg-coffee-50 transition-colors">
                                                    <User className="w-4 h-4" /> Thông tin
                                                </Link>
                                                {(user.role === 'Admin' || user.role === 'Employee') && (
                                                    <Link to="/admin" className="flex items-center gap-3 px-4 py-3 text-xs uppercase tracking-widest text-coffee-600 hover:bg-coffee-50 transition-colors">
                                                        <Settings className="w-4 h-4" /> Quản lý
                                                    </Link>
                                                )}
                                                <Link to="/orders" className="flex items-center gap-3 px-4 py-3 text-xs uppercase tracking-widest text-coffee-600 hover:bg-coffee-50 transition-colors">
                                                    <ReceiptText className="w-4 h-4" /> Hóa đơn
                                                </Link>
                                                <div className="h-[1px] bg-coffee-100 my-2" />
                                                <button
                                                    onClick={logout}
                                                    className="w-full flex items-center gap-3 px-4 py-3 text-xs uppercase tracking-widest text-red-600 hover:bg-red-50 transition-colors"
                                                >
                                                    <LogOut className="w-4 h-4" /> Đăng xuất
                                                </button>
                                            </motion.div>
                                        )}
                                    </AnimatePresence>
                                </>
                            ) : (
                                <Link to="/login" className="flex items-center gap-2 text-coffee-600 hover:text-coffee-900 font-medium text-xs uppercase tracking-widest group">
                                    <LogIn className="w-5 h-5 group-hover:translate-x-1 transition-transform" />
                                    <span className="hidden sm:inline">Đăng nhập</span>
                                </Link>
                            )}
                        </div>

                        {/* Cart */}
                        <Link to="/cart" className="relative text-coffee-600 hover:text-coffee-900 transition-transform duration-300 hover:scale-110">
                            <ShoppingCart className="w-5 h-5" />
                            {totalItems > 0 && (
                                <span className="absolute -top-2 -right-2 bg-coffee-300 text-white text-[10px] w-4 h-4 flex items-center justify-center font-bold rounded-full">
                                    {totalItems}
                                </span>
                            )}
                        </Link>

                        {/* Mobile Menu Toggle */}
                        <button
                            className="lg:hidden text-coffee-700"
                            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                        >
                            {isMobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
                        </button>
                    </div>
                </div>

                {/* Mobile Menu */}
                <AnimatePresence>
                    {isMobileMenuOpen && (
                        <motion.div
                            initial={{ opacity: 0, height: 0 }}
                            animate={{ opacity: 1, height: 'auto' }}
                            exit={{ opacity: 0, height: 0 }}
                            className="lg:hidden bg-white border-t border-coffee-100 overflow-hidden"
                        >
                            <div className="flex flex-col p-6 gap-6">
                                {navLinks.map((link) => (
                                    <Link
                                        key={link.path}
                                        to={link.path}
                                        onClick={() => setIsMobileMenuOpen(false)}
                                        className="text-lg font-display font-medium text-coffee-900"
                                    >
                                        {link.name}
                                    </Link>
                                ))}
                            </div>
                        </motion.div>
                    )}
                </AnimatePresence>
            </header>

            {/* Spacer for non-absolute positioning if needed, 
          but usually we use mt-20 on main content */}
        </>
    );
};

export default Navbar;
