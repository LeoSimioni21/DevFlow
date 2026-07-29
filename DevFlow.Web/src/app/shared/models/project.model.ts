export type ProjectStatus = 'in-progress' | 'attention' | 'critical';

export type ProjectIconType = 'code' | 'mobile' | 'devops' | 'api';

export interface Project {
  id: string;
  name: string;
  description: string;
  icon: ProjectIconType;
  progress: number;
  deadline: string;
  hasDeadline: boolean;
  status: ProjectStatus;
  tasksCount: number;
}
