import { useEffect, useState } from 'react';
import { getLatestMessages } from '../api/telemetryApi';
import type { TelemetryMessage } from '../api/telemetryApi';

type DeviceDetailsProps = {
    onBack?: () => void;
};

export function DeviceDetails({ onBack }: DeviceDetailsProps) {
    const [messages, setMessages] = useState<TelemetryMessage[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getLatestMessages(10)
            .then(setMessages)
            .catch(console.error)
            .finally(() => setLoading(false));
    }, []);

    const latest = messages[0];

    return (
        <>
            {/* Top Navigation Bar */}
            <header className="fixed top-0 left-0 w-full z-50 bg-surface/60 backdrop-blur-xl dark:bg-surface-container/60 border-b border-outline-variant/20 shadow-sm flex justify-between items-center px-margin-mobile h-16">
                <div className="flex items-center gap-3">
                    <button className="p-2 -ml-2 text-on-surface-variant cursor-pointer active:scale-95 duration-200">
                        <span onClick={onBack} className="material-symbols-outlined">arrow_back</span>
                    </button>
                    <div className="flex flex-col">
                        <span className="font-headline-md text-headline-md font-bold text-secondary">
                            {latest?.deviceId ?? 'Sensor'}
                        </span>
                    </div>
                </div>
                <div className="flex items-center gap-4">
                    <span className="material-symbols-outlined text-primary">sensors</span>
                </div>
            </header>

            <main className="pt-16 px-margin-mobile space-y-6">
                {/* Breadcrumbs & Status */}
                <section className="flex flex-col gap-2">
                    <nav className="flex items-center text-on-surface-variant/60 font-label-caps text-label-caps space-x-2">
                        <span>Devices</span>
                        <span className="material-symbols-outlined text-[14px]">chevron_right</span>
                        <span className="text-on-surface-variant">Temperature Sensors</span>
                    </nav>
                    <div className="flex items-center gap-2">
                        <div className="w-2.5 h-2.5 rounded-full bg-secondary status-pulse"></div>
                        <span className="font-label-caps text-label-caps text-secondary uppercase tracking-widest">Activo</span>
                    </div>
                </section>

                {/* Temperature Metric Card */}
                <section className="grid grid-cols-1 gap-4">
                    <div className="glass-card rounded-xl p-6 flex items-center justify-between">
                        <div>
                            <p className="font-label-caps text-label-caps text-on-surface-variant mb-1">Temperatura</p>
                            <p className="font-data-mono text-display-lg text-on-surface">
                                {loading ? '...' : (latest?.temperatureC.toFixed(1) ?? '--')}
                                <span className="text-headline-sm text-primary opacity-60">°C</span>
                            </p>
                            <p className="text-xs text-on-surface-variant mt-1">
                                Humedad: {loading ? '...' : (latest?.humidityPct.toFixed(0) ?? '--')}%
                            </p>
                        </div>
                        <div className="flex flex-col items-end">
                            <span className="material-symbols-outlined text-secondary text-4xl" style={{ fontVariationSettings: "'FILL' 1" }}>device_thermostat</span>
                            <span className={`font-label-caps text-label-caps mt-2 ${latest?.buzzerActive ? 'text-error' : 'text-secondary'}`}>
                                {latest?.buzzerActive ? '? Buzzer ON' : 'Normal'}
                            </span>
                        </div>
                    </div>
                </section>

                {/* Telemetry History Table */}
                <section className="glass-card rounded-xl overflow-hidden mb-8">
                    <div className="px-6 py-4 flex justify-between items-center border-b border-outline-variant/20">
                        <h3 className="font-headline-sm text-headline-sm">History</h3>
                    </div>
                    <div className="overflow-x-auto hide-scrollbar">
                        <table className="w-full text-left min-w-[400px]">
                            <thead className="bg-surface-container-high/40 font-label-caps text-label-caps text-on-surface-variant">
                                <tr>
                                    <th className="px-6 py-3 font-semibold">Timestamp</th>
                                    <th className="px-6 py-3 font-semibold text-center">Seq ID</th>
                                    <th className="px-6 py-3 font-semibold text-right">Temp</th>
                                    <th className="px-6 py-3 font-semibold text-right">Hum</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-outline-variant/10 font-body-md text-body-md">
                                {loading && (
                                    <tr><td colSpan={4} className="px-6 py-4 text-center text-on-surface-variant">Cargando...</td></tr>
                                )}
                                {!loading && messages.length === 0 && (
                                    <tr><td colSpan={4} className="px-6 py-4 text-center text-on-surface-variant">Sin datos</td></tr>
                                )}
                                {messages.map((m) => (
                                    <tr key={`${m.sequenceNumber}-${m.timestamp}`} className="hover:bg-surface-variant/20 transition-colors">
                                        <td className="px-6 py-4 text-on-surface">
                                            {new Date(m.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
                                        </td>
                                        <td className="px-6 py-4 text-center font-data-mono text-xs opacity-60">#{m.sequenceNumber}</td>
                                        <td className="px-6 py-4 text-right text-secondary font-medium">{m.temperatureC.toFixed(1)}°C</td>
                                        <td className="px-6 py-4 text-right text-on-surface">{m.humidityPct.toFixed(0)}%</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                    <div className="px-6 py-3 bg-surface-container/20 text-center">
                        <p className="font-label-caps text-label-caps text-on-surface-variant/40">Last 10 records</p>
                    </div>
                </section>
            </main>

            {/* Bottom Navigation Bar */}
            <nav className="fixed bottom-0 w-full z-50 bg-surface/60 backdrop-blur-xl dark:bg-surface-container/60 border-t border-outline-variant/20 shadow-lg flex justify-around items-center h-20 px-4 pb-safe md:hidden">
                <a className="flex flex-col items-center justify-center text-on-surface-variant opacity-70 hover:opacity-100 transition-opacity active:scale-90 duration-200" href="#">
                    <span className="material-symbols-outlined">dashboard</span>
                    <span className="font-label-caps text-label-caps mt-1">Overview</span>
                </a>
                <a className="flex flex-col items-center justify-center bg-secondary-container/20 text-secondary rounded-xl px-4 py-1 active:scale-90 duration-200" href="#">
                    <span className="material-symbols-outlined" style={{ fontVariationSettings: "'FILL' 1" }}>router</span>
                    <span className="font-label-caps text-label-caps mt-1">Devices</span>
                </a>
                <a className="flex flex-col items-center justify-center text-on-surface-variant opacity-70 hover:opacity-100 transition-opacity active:scale-90 duration-200" href="#">
                    <span className="material-symbols-outlined">analytics</span>
                    <span className="font-label-caps text-label-caps mt-1">Analytics</span>
                </a>
                <a className="flex flex-col items-center justify-center text-on-surface-variant opacity-70 hover:opacity-100 transition-opacity active:scale-90 duration-200" href="#">
                    <span className="material-symbols-outlined">settings</span>
                    <span className="font-label-caps text-label-caps mt-1">Settings</span>
                </a>
            </nav>
        </>
    );
}
