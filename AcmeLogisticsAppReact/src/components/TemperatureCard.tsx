type TemperatureCardProps = {
  value: string;
  unit?: string;
  active: string;
  label?: string;
  icon?: string;
  onClick?: () => void;
};

export function TemperatureCard({
  value,
  unit = '°C',
  active,
  label = 'TEMPERATURE',
  icon = 'thermostat',
  onClick,
}: TemperatureCardProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="glass-card rounded-xl p-5 flex flex-col justify-between aspect-square group active:scale-95 transition-transform duration-200 text-left"
    >
      <div className="flex justify-between items-start">
        <div className="p-2 bg-secondary/10 rounded-lg text-secondary">
          <span className="material-symbols-outlined" data-icon={icon}>{icon}</span>
        </div>
        <span className="status-pill bg-secondary/20 text-secondary border border-secondary/30">{active}</span>
      </div>
      <div>
        <p className="font-label-caps text-label-caps text-on-surface-variant opacity-70 mb-1">{label}</p>
        <div className="flex items-baseline gap-1">
          <span className="font-data-mono text-display-lg text-on-surface">{value}</span>
          <span className="font-body-lg text-body-lg text-secondary">{unit}</span>
        </div>
      </div>
    </button>
  );
}
