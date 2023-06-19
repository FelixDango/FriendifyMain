import { Component, OnInit, ViewChild } from '@angular/core';
import { UIChart } from 'primeng/chart';
import { AdminData } from '../../models/admin-data';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  // Declare variables for the admin data and the charts
  adminData: AdminData | null = null;
  registrationChart: any;
  postsChart: any;
  genderChart: any;
  @ViewChild('chart') chart!: UIChart;


  data: any;
  options: any;
 

  // Inject the AdminService in the constructor
  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    // Call the getAdminData method on initialization
    this.getAdminData();
    setTimeout(() => {
      this.chart.refresh();
    }, 100);
    
  }

  // Define a method to get the admin data from the service
  getAdminData(selectedTime?: Date): void {
    // Call the getAdminData method of the service and subscribe to the result
    this.adminService.getAdminData(selectedTime).subscribe(
      (data: AdminData) => {
        // Assign the data to the adminData variable
        this.adminData = data;
        // Create the charts using the data
        this.createRegistrationChart();
        this.createPostsChart();
        this.createGenderChart();
      },
      (error: any) => {
        // Handle any possible errors
        console.error(error);
        alert(error.message);
      }
    );
  }

  // Define a method to create the registration chart using Chart.js
  createRegistrationChart(): void {
    // Check if the admin data is available
    if (this.adminData) {
      // Get the registration dates from the admin data
      const dates = this.adminData.registrationDates;
      // Create an array of labels for each month in the selected time range
      const labels = dates.map(date => date.toLocaleString('en-US', { month: 'short' }));
      // Create an array of values for each month in the selected time range
      const values = dates.map(date => date.getDate());
      // Create a chart object with options and data
      this.registrationChart = {
        type: 'line', // Use a line chart
        options: {
          responsive: true, // Make it responsive
          plugins: {
            legend: {
              display: false // Hide the legend
            },
            title: {
              display: true, // Show the title
              text: 'New Registrations' // Set the title text
            }
          }
        },
        data: {
          labels, // Use the labels array for the x-axis
          datasets: [
            {
              label: 'Registrations', // Set the label for the dataset
              data: values, // Use the values array for the y-axis
              fill: false, // Do not fill the area under the line
              borderColor: '#42A5F5', // Set the line color to blue
              tension: 0.4 // Set the line tension to smooth it out
            }
          ]
        }
      };
    }
  }

  // Define a method to create the posts chart using Chart.js
  createPostsChart(): void {
    // Check if the admin data is available
    if (this.adminData) {
      // Get the posts creation dates from the admin data
      const dates = this.adminData.postsCreationDates;
      // Create an array of labels for each day in the selected time range
      const labels = dates.map(date => date.toLocaleString('en-US', { day: 'numeric' }));
      // Create an array of values for each day in the selected time range
      const values = dates.map(date => date.getHours());
      // Create a chart object with options and data
      this.postsChart = {
        type: 'bar', // Use a bar chart
        options: {
          responsive: true, // Make it responsive
          plugins: {
            legend: {
              display: false // Hide the legend
            },
            title: {
              display: true, // Show the title
              text: 'New Posts' // Set the title text
            }
          }
        },
        data: {
          labels, // Use the labels array for the x-axis
          datasets: [
            {
              label: 'Posts', // Set the label for the dataset
              data: values, // Use the values array for the y-axis
              backgroundColor: '#66BB6A', // Set the bar color to green
              borderColor: '#66BB6A', // Set the border color to green
              borderWidth: 1 // Set the border width to 1 pixel
            }
          ]
        }
      };
    }
  }

  // Define a method to create the gender chart using Chart.js
  createGenderChart(): void {
    // Check if the admin data is available
    if (this.adminData) {
      // Get the male and female counts from the admin data
      const maleCount = this.adminData.maleCount;
      const femaleCount = this.adminData.femaleCount;
      // Create an array of labels for the genders
      const labels = ['Male', 'Female'];
      // Create an array of values for the genders
      const values = [maleCount, femaleCount];
      // Create a chart object with options and data
      this.genderChart = {
        type: 'pie', // Use a pie chart
        options: {
          responsive: true, // Make it responsive
          plugins: {
            legend: {
              display: true, // Show the legend
              position: 'right' // Position the legend on the right
            },
            title: {
              display: true, // Show the title
              text: 'Gender Distribution' // Set the title text
            }
          }
        },
        data: {
          labels, // Use the labels array for the slices
          datasets: [
            {
              label: 'Gender', // Set the label for the dataset
              data: values, // Use the values array for the slices
              backgroundColor: ['#FF6384', '#36A2EB'], // Set the colors for the slices to pink and blue
              hoverOffset: 4 // Set the hover offset to 4 pixels
            }
          ]
        }
      };
    }
  }

}