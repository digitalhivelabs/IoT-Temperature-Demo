import './App.css'
import { useEffect, useState } from 'react';
import { Header } from './components/Header';
import { AlertCard } from './components/AlertCard';
import { TemperatureCard } from './components/TemperatureCard';
import { DeviceDetails } from './components/DeviceDetails';
import { getActiveAlerts, getSummary } from './api/telemetryApi';
import type { AlertMessage, TelemetrySummary } from './api/telemetryApi';

function App() {
  const [selectedDevice, setSelectedDevice] = useState(false);
  const [summary, setSummary] = useState<TelemetrySummary | null>(null);
  const [alerts, setAlerts] = useState<AlertMessage[]>([]);

  useEffect(() => {
    getSummary().then(setSummary).catch(console.error);
    getActiveAlerts().then(setAlerts).catch(console.error);
  }, []);

  const handleOpenDetails = () => setSelectedDevice(true);
  const handleCloseDetails = () => setSelectedDevice(false);

  return (
    <>
      <Header />
      {selectedDevice ? (
        <DeviceDetails onBack={handleCloseDetails} />
      ) : (
        <>
          <main className="mt-20 px-margin-mobile">
            <div className="mb-8">
              <h1 className="font-display-lg text-display-lg text-on-surface">Overview</h1>
              <p className="font-body-md text-body-md text-on-surface-variant">System status: All gateways operational</p>
            </div>

            <div className="grid grid-cols-2 gap-4 mb-10">
              <TemperatureCard
                value={summary?.lastTemperature != null ? summary.lastTemperature.toFixed(1) : '--'}
                unit="C"
                active={summary ? `${summary.alertsCount} Alertas` : '...'}
                label="TEMPERATURA"
                onClick={handleOpenDetails}
              />
            </div>

            <section className="mb-6">
              <div className="flex justify-between items-end mb-4">
                <h2 className="font-headline-sm text-headline-sm text-on-surface">Alertas Recientes</h2>
                <button className="font-label-caps text-label-caps text-secondary uppercase tracking-widest">Ver Todas</button>
              </div>
              <div className="space-y-3">
                {alerts.length === 0 && (
                  <p className="font-body-md text-body-md text-on-surface-variant">Sin alertas activas.</p>
                )}
                {alerts.map((alert, i) => (
                  <AlertCard
                    key={alert.blobName ?? `${alert.deviceId}-${i}`}
                    title={alert.message}
                    subtitle={`${alert.deviceId} - ${alert.temperatureC}°C`}
                    time={new Date(alert.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    status={alert.acknowledged ? 'RESUELTO' : 'ACTIVO'}
                    variant={alert.acknowledged ? 'resolved' : 'critical'}
                    icon={alert.acknowledged ? 'check_circle' : 'warning'}
                  />
                ))}
              </div>
            </section>
          </main>

          <nav className="bg-surface/60 backdrop-blur-xl dark:bg-surface-container/60 fixed bottom-0 w-full z-50 border-t border-outline-variant/20 shadow-lg flex justify-around items-center h-20 px-4 pb-safe">
            <div className="flex flex-col items-center justify-center bg-secondary-container/20 text-secondary dark:text-secondary-fixed rounded-xl px-4 py-1 active:scale-90 duration-200 hover:opacity-100 transition-opacity">
              <span className="material-symbols-outlined text-[24px]">dashboard</span>
              <span className="font-label-caps text-label-caps mt-1">Overview</span>
            </div>
            <div className="flex flex-col items-center justify-center text-on-surface-variant opacity-70 active:scale-90 duration-200 hover:opacity-100 transition-opacity">
              <span className="material-symbols-outlined text-[24px]">router</span>
              <span className="font-label-caps text-label-caps mt-1">Devices</span>
            </div>
            <div className="flex flex-col items-center justify-center text-on-surface-variant opacity-70 active:scale-90 duration-200 hover:opacity-100 transition-opacity">
              <span className="material-symbols-outlined text-[24px]">analytics</span>
              <span className="font-label-caps text-label-caps mt-1">Analytics</span>
            </div>
            <div className="flex flex-col items-center justify-center text-on-surface-variant opacity-70 active:scale-90 duration-200 hover:opacity-100 transition-opacity">
              <span className="material-symbols-outlined text-[24px]">settings</span>
              <span className="font-label-caps text-label-caps mt-1">Settings</span>
            </div>
          </nav>
        </>
      )}
    </>
  )
}

export default App
