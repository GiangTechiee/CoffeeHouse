import { useState, useEffect } from 'react';
import { useNavigate, Link, useLocation } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Mail, Lock, ArrowRight, Loader2, AlertCircle, CheckCircle2, Eye, EyeOff } from 'lucide-react';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import type { ApiResponse } from '../types';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [showPassword, setShowPassword] = useState(false);
    const { login } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        if (location.state?.message) {
            setSuccessMessage(location.state.message);
        }
    }, [location]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        setSuccessMessage(null);

        try {
            const response = await api.post<ApiResponse<any>>('/v1/auth/login', {
                email, // Matches Backend LoginCommand parameter name
                password
            });

            if (response.data.success) {
                const { token, ...userData } = response.data.data;
                login(token, userData);
                navigate('/');
            } else {
                setError(response.data.message || 'Đăng nhập thất bại');
            }
        } catch (err: any) {
            setError(err.response?.data?.message || 'Đã có lỗi xảy ra. Vui lòng thử lại.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-coffee-50 flex items-center justify-center p-6 relative overflow-hidden">
            {/* background elements */}
            <div className="absolute top-0 left-0 w-full h-full overflow-hidden z-0 pointer-events-none">
                <div className="absolute top-[-10%] left-[-10%] w-[40%] aspect-square bg-coffee-100/50 rounded-full blur-3xl animate-pulse" />
                <div className="absolute bottom-[-10%] right-[-10%] w-[30%] aspect-square bg-coffee-200/30 rounded-full blur-3xl animate-pulse" />
            </div>

            <motion.div
                initial={{ opacity: 0, scale: 0.9 }}
                animate={{ opacity: 1, scale: 1 }}
                className="w-full max-w-md bg-white p-12 shadow-2xl relative z-10"
            >
                <div className="text-center mb-12">
                    <span className="text-[10px] uppercase tracking-[0.4em] text-coffee-400 font-bold block mb-4">Chào mừng trở lại</span>
                    <h1 className="text-5xl font-display text-coffee-800">Đăng nhập</h1>
                </div>

                {error && (
                    <motion.div
                        initial={{ opacity: 0, y: -10 }}
                        animate={{ opacity: 1, y: 0 }}
                        className="mb-8 p-4 bg-red-50 text-red-600 text-xs flex items-center gap-3 border-l-4 border-red-500"
                    >
                        <AlertCircle className="w-4 h-4 shrink-0" />
                        {error}
                    </motion.div>
                )}

                {successMessage && (
                    <motion.div
                        initial={{ opacity: 0, y: -10 }}
                        animate={{ opacity: 1, y: 0 }}
                        className="mb-8 p-4 bg-green-50 text-green-700 text-xs flex items-center gap-3 border-l-4 border-green-500"
                    >
                        <CheckCircle2 className="w-4 h-4 shrink-0" />
                        {successMessage}
                    </motion.div>
                )}

                <form onSubmit={handleSubmit} className="space-y-8">
                    <div className="space-y-6">
                        <div className="relative group">
                            <label className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold mb-2 block">Email / Tên đăng nhập</label>
                            <div className="relative">
                                <input
                                    type="text"
                                    required
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    className="input-field pl-10 h-14"
                                    placeholder="your@email.com"
                                />
                                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-coffee-300 group-focus-within:text-coffee-700 transition-colors" />
                            </div>
                        </div>

                        <div className="relative group">
                            <div className="flex justify-between mb-2">
                                <label className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold block">Mật khẩu</label>
                                <a href="#" className="text-[10px] uppercase tracking-widest text-coffee-400 hover:text-coffee-800 transition-colors">Quên mật khẩu?</a>
                            </div>
                            <div className="relative">
                                <input
                                    type={showPassword ? "text" : "password"}
                                    required
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    className="input-field pl-10 pr-12 h-14"
                                    placeholder="••••••••"
                                />
                                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-coffee-300 group-focus-within:text-coffee-700 transition-colors" />
                                <button
                                    type="button"
                                    onClick={() => setShowPassword(!showPassword)}
                                    className="absolute right-3 top-1/2 -translate-y-1/2 text-coffee-300 hover:text-coffee-700 transition-colors"
                                >
                                    {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                                </button>
                            </div>
                        </div>
                    </div>

                    <button
                        type="submit"
                        disabled={loading}
                        className="w-full bg-coffee-800 text-white h-16 uppercase tracking-[0.2em] text-[10px] font-bold hover:bg-coffee-900 transition-all flex items-center justify-center gap-4 group disabled:opacity-50"
                    >
                        {loading ? (
                            <Loader2 className="w-4 h-4 animate-spin" />
                        ) : (
                            <>
                                Đăng nhập
                                <ArrowRight className="w-4 h-4 transition-transform group-hover:translate-x-1" />
                            </>
                        )}
                    </button>
                </form>

                <div className="mt-12 text-center">
                    <p className="text-xs text-coffee-400 font-light">
                        Chưa có tài khoản?{' '}
                        <Link to="/register" className="text-coffee-800 font-bold hover:underline">Đăng ký ngay</Link>
                    </p>
                </div>
            </motion.div>
        </div>
    );
};

export default Login;
