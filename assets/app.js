// Change the API base URL if needed
const API_BASE = "https://library-api.up.railway.app/api";

async function fetchDepartments() {
    const res = await fetch(`${API_BASE}/Department`);
    const data = await res.json();
    const list = document.getElementById('departments');
    list.innerHTML = '';
    data.forEach(dept => {
        const li = document.createElement('li');
        li.textContent = `${dept.departmentCode}: ${dept.departmentName} (Manager: ${dept.managerName || 'None'})`;
        list.appendChild(li);
    });
}

async function fetchEmployees() {
    const res = await fetch(`${API_BASE}/Employee`);
    const data = await res.json();
    const list = document.getElementById('employees');
    list.innerHTML = '';
    data.forEach(emp => {
        li = document.createElement('li');
        li.textContent = `${emp.id}: ${emp.firstName} ${emp.lastName} (Dept: ${emp.departmentCode || 'None'}) (Manager: ${emp.managerName || 'None'})`;
        list.appendChild(li);
    });
}

async function fetchProjects() {
    const res = await fetch(`${API_BASE}/ProjectAssign`);
    const data = await res.json();
    const list = document.getElementById('projects');
    list.innerHTML = '';
    data.forEach(proj => {
        const li = document.createElement('li');
        li.textContent = `Assignment: ${proj.assignId}, Employee: ${proj.employeeNum}, Project: ${proj.projectCode}, Hours/Week: ${proj.hrsPerWeek}`;
        list.appendChild(li);
    });
}

fetchDepartments();
fetchEmployees();
fetchProjects();
