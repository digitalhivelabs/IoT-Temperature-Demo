type AlertVariant = 'critical' | 'resolved' | 'info';

type AlertCardProps = {
  title: string;
  subtitle: string;
  time: string;
  status: string;
  variant: AlertVariant;
  icon: string;
};

const variantStyles: Record<AlertVariant, { border: string; iconBg: string; iconColor: string; statusColor: string }> = {
  critical: {
    border: 'border-l-error',
    iconBg: 'bg-error-container/20',
    iconColor: 'text-error',
    statusColor: 'text-error',
  },
  resolved: {
    border: 'border-l-secondary',
    iconBg: 'bg-secondary-container/20',
    iconColor: 'text-secondary',
    statusColor: 'text-secondary',
  },
  info: {
    border: 'border-l-primary',
    iconBg: 'bg-primary-container/20',
    iconColor: 'text-primary',
    statusColor: 'text-primary',
  },
};

export function AlertCard({ title, subtitle, time, status, variant, icon }: AlertCardProps) {
  const styles = variantStyles[variant];

  return (
    <div className={`glass-card rounded-xl p-4 flex items-center justify-between ${styles.border}`}>
      <div className="flex items-center gap-3">
        <div className={`w-10 h-10 rounded-full ${styles.iconBg} flex items-center justify-center ${styles.iconColor}`}>
          <span className="material-symbols-outlined">{icon}</span>
        </div>
        <div>
          <h3 className="font-body-lg text-body-lg font-semibold text-on-surface">{title}</h3>
          <p className="font-label-caps text-[10px] text-on-surface-variant">{subtitle}</p>
        </div>
      </div>
      <div className="text-right">
        <span className="block font-label-caps text-label-caps text-on-surface-variant opacity-60">{time}</span>
        <span className={`font-label-caps text-[10px] font-bold ${styles.statusColor}`}>{status}</span>
      </div>
    </div>
  );
}
