using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.utilities
{
    public class ProfileFormData
    {
        public static List<string> UniversityList { get; } = new ()
        {
            "Babes-Bolyai University",
            "Technical University of Cluj-Napoca",
            "University of Bucharest",
            "Politehnica University of Bucharest",
            "Alexandru Ioan Cuza University",
            "West University of Timisoara",
            "University of Medicine and Pharmacy Cluj-Napoca",
            "Academy of Economic Studies Bucharest"
        };

        public static List<string> SkillSuggestions { get; } = new ()
        {
            // Languages
            "JavaScript", "TypeScript", "Python", "Java", "C#", "Go", "R", "Julia",

            // Frontend
            "HTML", "CSS", "SCSS", "Tailwind",

            // Frameworks & Libraries
            "React", "Angular", "Vue.js", "Svelte", "Node.js", "Spring Boot", "ASP.NET", "Django", "FastAPI",

            // Build Tools
            "Webpack", "Vite", "Parcel",

            // Version Control
            "Git", "GitHub",

            // Testing
            "Jest", "Cypress", "Selenium", "JUnit", "NUnit", "pytest",

            // APIs & Communication
            "REST", "GraphQL", "gRPC", "Postman",

            // Databases
            "SQL", "PostgreSQL", "MySQL", "MongoDB", "Redis", "BigQuery",

            // DevOps / Cloud
            "Docker", "Podman", "Kubernetes", "Docker Swarm", "OpenShift",
            "Jenkins", "GitHub Actions", "GitLab CI", "CircleCI",
            "AWS", "Azure", "Google Cloud",
            "Terraform", "Ansible", "Pulumi",
            "Prometheus", "Grafana", "Datadog",

            // Design & UX
            "Figma", "Adobe XD", "Zeplin", "Sketch", "InVision", "Marvel", "Axure",
            "Figma Prototyping", "Typography", "Color Theory", "Grid Systems",
            "Storybook",

            // Research & UX Methods
            "Interviews", "Surveys", "Usability Testing",

            // Analytics & BI
            "Google Analytics", "Hotjar", "Mixpanel",
            "Power BI", "Tableau", "Looker",
            "Excel", "Google Sheets", "OpenRefine",

            // Data Science & AI
            "Pandas", "NumPy", "TensorFlow", "PyTorch", "scikit-learn", "Keras",
            "Apache Spark", "MLflow", "Hugging Face",
            "OpenCV", "NLTK", "spaCy",
            "Descriptive Statistics", "Regression", "Hypothesis Testing",
            "Linear Algebra", "Calculus", "Probability", "Statistics",

            // Security
            "Firewalls", "VPN", "IDS/IPS", "TCP/IP",
            "Metasploit", "Burp Suite", "Nmap",
            "Splunk", "IBM QRadar", "Microsoft Sentinel",
            "AES", "RSA", "PKI", "TLS/SSL",
            "ISO 27001", "GDPR", "NIST", "SOC 2",
            "Forensics", "Malware Analysis", "DFIR",

            // Project Management
            "Scrum", "Kanban", "Agile", "Waterfall",
            "Jira", "Trello", "Asana",
            "Risk Assessment", "Mitigation Planning",
            "Stakeholder Management", "Reporting",
            "Presentations", "Cost Estimation", "Budget Tracking",
            "MS Project"
        };
    }
}
