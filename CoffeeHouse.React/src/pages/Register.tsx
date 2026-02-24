import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { User, Mail, Lock, Phone, MapPin, ArrowRight, Loader2, AlertCircle, CheckCircle2, Eye, EyeOff } from 'lucide-react';
import api from '../services/api';
import type { ApiResponse } from '../types';

const Register = () => {
    const [formData, setFormData] = useState({
        email: '',
        fullName: '',
        phoneNumber: '',
        address: '',
        password: '',
        confirmPassword: ''
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData(prev => ({
            ...prev,
            [e.target.name]: e.target.value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (formData.password !== formData.confirmPassword) {
            setError('Mật khẩu xác nhận không khớp');
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const response = await api.post<ApiResponse<any>>('/v1/auth/register', formData);

            if (response.data.success) {
                // Registration successful, redirect to login
                navigate('/login', { state: { message: 'Đăng ký thành công! Vui lòng đăng nhập.' } });
            } else {
                setError(response.data.error?.message || 'Đăng ký thất bại');
            }
        } catch (err: any) {
            console.error('Registration error details:', err.response?.data);

            const responseData = err.response?.data;

            // 1. Try custom ApiResponse structure: error { message, details }
            if (responseData?.error) {
                const apiError = responseData.error;
                if (apiError.details && typeof apiError.details === 'object') {
                    const detailMessages = Object.values(apiError.details)
                        .flat()
                        .filter(msg => typeof msg === 'string')
                        .join('. ');
                    setError(`${apiError.message}: ${detailMessages}`);
                } else {
                    setError(apiError.message || 'Đăng ký thất bại');
                }
            }
            // 2. Try default ASP.NET Core ProblemDetails: title, errors { ... }
            else if (responseData?.errors) {
                const detailMessages = Object.values(responseData.errors)
                    .flat()
                    .filter(msg => typeof msg === 'string')
                    .join('. ');
                setError(detailMessages || responseData.title || 'Dữ liệu không hợp lệ');
            }
            // 3. Try legacy or flat message
            else if (responseData?.message) {
                setError(responseData.message);
            }
            // 4. Fallback
            else {
                setError(err.response?.statusText || 'Đã có lỗi xảy ra. Vui lòng thử lại.');
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-coffee-50 flex items-center justify-center py-20 px-6 relative overflow-hidden">
            {/* background elements */}
            <div className="absolute top-0 left-0 w-full h-full overflow-hidden z-0 pointer-events-none">
                <div className="absolute top-[-10%] left-[-10%] w-[40%] aspect-square bg-coffee-100/50 rounded-full blur-3xl animate-pulse" />
                <div className="absolute bottom-[-10%] right-[-10%] w-[30%] aspect-square bg-coffee-200/30 rounded-full blur-3xl animate-pulse" />
            </div>

            <motion.div
                initial={{ opacity: 0, scale: 0.9 }}
                animate={{ opacity: 1, scale: 1 }}
                className="w-full max-w-4xl bg-white p-12 shadow-2xl relative z-10"
            >
                <div className="text-center mb-12">
                    <span className="text-[10px] uppercase tracking-[0.4em] text-coffee-400 font-bold block mb-4">Tham gia cùng chúng tôi</span>
                    <h1 className="text-5xl font-display text-coffee-800">Đăng ký</h1>
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

                <form onSubmit={handleSubmit} className="space-y-8">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                        {/* Left Column */}
                        <div className="space-y-6">
                            <div className="relative group">
                                <label className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold mb-2 block">Email</label>
                                <div className="relative">
                                    <input
                                        type="email"
                                        name="email"
                                        required
                                        value={formData.email}
                                        onChange={handleChange}
                                        className="input-field pl-10 h-14"
                                        placeholder="example@email.com"
                                    />
                                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-coffee-300 group-focus-within:text-coffee-700 transition-colors" />
                                </div>
                                <p className="text-[9px] text-coffee-300 mt-1 italic">* Email sẽ được dùng làm tên đăng nhập</p>
                            </div>

                            <div className="relative group">
                                <label className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold mb-2 block">Họ và tên</label>
                                <div className="relative">
                                    <input
                                        type="text"
                                        name="fullName"
                                        required
                                        value={formData.fullName}
                                        onChange={handleChange}
                                        className="input-field pl-10 h-14"
                                        placeholder="Nguyễn Văn A"
                                    />
                                    <User className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-coffee-300 group-focus-within:text-coffee-700 transition-colors" />
                                </div>
                            </div>

                            <div className="relative group">
                                <label className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold mb-2 block">Số điện thoại</label>
                                <div className="relative">
                                    <input
                                        type="tel"
                                        name="phoneNumber"
                                        required
                                        value={formData.phoneNumber}
                                        onChange={handleChange}
                                        className="input-field pl-10 h-14"
                                        placeholder="0123456789"
                                    />
                                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-coffee-300 group-focus-within:text-coffee-700 transition-colors" />
                                </div>
                            </div>
                        </div>

                        {/* Right Column */}
                        <div className="space-y-6">
                            <div className="relative group">
                                <label className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold mb-2 block">Địa chỉ</label>
                                <div className="relative">
                                    <input
                                        type="text"
                                        name="address"
                                        required
                                        value={formData.address}
                                        onChange={handleChange}
                                        className="input-field pl-10 h-14"
                                        placeholder="Hà Nội, Việt Nam"
                                    />
                                    <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-coffee-300 group-focus-within:text-coffee-700 transition-colors" />
                                </div>
                            </div>

                            <div className="relative group">
                                <label className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold mb-2 block">Mật khẩu</label>
                                <div className="relative">
                                    <input
                                        type={showPassword ? "text" : "password"}
                                        name="password"
                                        required
                                        value={formData.password}
                                        onChange={handleChange}
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

                                {/* Password Criteria */}
                                <div className="mt-4 space-y-2">
                                    {[
                                        { label: 'Ít nhất 8 ký tự', met: formData.password.length >= 8 },
                                        { label: 'Chứa chữ hoa (A-Z)', met: /[A-Z]/.test(formData.password) },
                                        { label: 'Chứa chữ thường (a-z)', met: /[a-z]/.test(formData.password) },
                                        { label: 'Chứa chữ số (0-9)', met: /\d/.test(formData.password) },
                                        { label: 'Ký tự đặc biệt (!@#$%^&*)', met: /[^A-Za-z0-9]/.test(formData.password) },
                                    ].map((criterion, index) => (
                                        <motion.div
                                            key={index}
                                            initial={false}
                                            animate={{ color: criterion.met ? '#2D6A4F' : '#9CA3AF' }}
                                            className="flex items-center gap-2 text-[10px] font-medium"
                                        >
                                            <div className="relative w-3 h-3 flex items-center justify-center">
                                                <motion.div
                                                    initial={{ scale: 0, opacity: 0 }}
                                                    animate={{
                                                        scale: criterion.met ? 1 : 0,
                                                        opacity: criterion.met ? 1 : 0
                                                    }}
                                                    className="absolute"
                                                >
                                                    <CheckCircle2 className="w-3 h-3 text-green-600" />
                                                </motion.div>
                                                <motion.div
                                                    animate={{ opacity: criterion.met ? 0 : 1 }}
                                                    className="w-1 h-1 bg-coffee-200 rounded-full"
                                                />
                                            </div>
                                            <span className={criterion.met ? "text-green-600 transition-all" : ""}>
                                                {criterion.label}
                                            </span>
                                        </motion.div>
                                    ))}
                                </div>
                            </div>

                            <div className="relative group">
                                <label className="text-[10px] uppercase tracking-widest text-coffee-400 font-bold mb-2 block">Xác nhận mật khẩu</label>
                                <div className="relative">
                                    <input
                                        type={showConfirmPassword ? "text" : "password"}
                                        name="confirmPassword"
                                        required
                                        value={formData.confirmPassword}
                                        onChange={handleChange}
                                        className="input-field pl-10 pr-12 h-14"
                                        placeholder="••••••••"
                                    />
                                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-coffee-300 group-focus-within:text-coffee-700 transition-colors" />
                                    <button
                                        type="button"
                                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                        className="absolute right-3 top-1/2 -translate-y-1/2 text-coffee-300 hover:text-coffee-700 transition-colors"
                                    >
                                        {showConfirmPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                                    </button>
                                </div>
                                {formData.confirmPassword && (
                                    <motion.p
                                        initial={{ opacity: 0, y: -5 }}
                                        animate={{ opacity: 1, y: 0 }}
                                        className={`text-[9px] mt-2 font-bold uppercase tracking-tighter ${formData.password === formData.confirmPassword ? 'text-green-600' : 'text-red-500'}`}
                                    >
                                        {formData.password === formData.confirmPassword ? '✓ Mật khẩu đã khớp' : '✗ Mật khẩu chưa khớp'}
                                    </motion.p>
                                )}
                            </div>
                        </div>
                    </div>

                    <button
                        type="submit"
                        disabled={loading}
                        className="w-full bg-coffee-800 text-white h-16 uppercase tracking-[0.2em] text-[10px] font-bold hover:bg-coffee-900 transition-all flex items-center justify-center gap-4 group disabled:opacity-50 mt-12"
                    >
                        {loading ? (
                            <Loader2 className="w-4 h-4 animate-spin" />
                        ) : (
                            <>
                                Đăng ký
                                <ArrowRight className="w-4 h-4 transition-transform group-hover:translate-x-1" />
                            </>
                        )}
                    </button>
                </form>

                <div className="mt-12 text-center">
                    <p className="text-xs text-coffee-400 font-light">
                        Đã có tài khoản?{' '}
                        <Link to="/login" className="text-coffee-800 font-bold hover:underline">Đăng nhập ngay</Link>
                    </p>
                </div>
            </motion.div>
        </div>
    );
};

export default Register;
