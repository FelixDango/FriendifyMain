import { Component, OnInit, ViewChild } from '@angular/core';
import { UIChart } from 'primeng/chart';
import { AdminData } from '../../models/admin-data';
import { AdminService } from '../../services/admin.service';
import { MenuItem, SelectItem } from 'primeng/api';
import { LayoutService } from '../../layout/service/app.layout.service';
import { jsPDF } from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
})

export class AdminComponent implements OnInit {

  // Declare variables for the admin data and the charts
  adminData: AdminData | null = null;
  registrationChart: any;
  postsChart: any;
  genderChart: any;

  timeframes: SelectItem[] = [
    { label: '1 day', value: 1 },
    { label: '1 week', value: 7 },
    { label: '1 month', value: 30 },
    { label: '1 year', value: 365 },
    { label: '5 years', value: 1825 }
  ];

  @ViewChild('chart') chart!: UIChart;
  selectedTimeframe: number = 7; // Default to 1 week

  data: any;
  options: any;
 

  // Inject the AdminService in the constructor
  constructor(private adminService: AdminService, public layoutService: LayoutService) { }

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
      // Create an empty object to store the counts for each date
      const tempcounts: { [key: string]: number } = {};

      // Loop through the registration dates array and increment the count for each date
      this.adminData.registrationDates.forEach(date => {
        // Create a Date object from the date variable
        const dateObject = new Date(date);
        // Get the date as a string in the format yyyy-mm-dd
        const dateString = dateObject.toISOString().slice(0, 10);
        // If the date is not in the counts object, initialize it to zero
        if (!tempcounts[dateString]) {
          tempcounts[dateString] = 0;
        }
        // Increment the count for the date by one
        tempcounts[dateString]++;
      });

      // Create an array of values for each date with the registration count
      const values = Object.entries(tempcounts).map(([date, count]) => ({ date, count }));

      // Map the values array to an array of numbers, using only the count property
      const counts = values.map(value => value.count);
      const labels = values.map(value => value.date);

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
              data: counts, // Use the counts array for the y-axis
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
      // Create an empty object to store the counts for each date
      const tempcounts: { [key: string]: number } = {};

      // Loop through the posts creation dates array and increment the count for each date
      this.adminData.postsCreationDates.forEach(date => {
        // Create a Date object from the date variable
        const dateObject = new Date(date);
        // Get the date as a string in the format yyyy-mm-dd
        const dateString = dateObject.toISOString().slice(0, 10);
        // If the date is not in the counts object, initialize it to zero
        if (!tempcounts[dateString]) {
          tempcounts[dateString] = 0;
        }
        // Increment the count for the date by one
        tempcounts[dateString]++;
      });

      // Create an array of values for each date with the post count
      const values = Object.entries(tempcounts).map(([date, count]) => ({ date, count }));

      // Map the values array to an array of numbers, using only the count property
      const counts = values.map(value => value.count);

      // Create an array of labels for each date in the selected time range
      const labels = values.map(value => value.date);

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
              data: counts, // Use the counts array for the y-axis
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
  onTimeframeChange(event: any): void {
    // Get the selected value from the event object
    const selectedValue = event.value;
    // Create a new Date object with the current date
    const currentDate = new Date();
    // Subtract the selected value (in days) from the current date
    currentDate.setDate(currentDate.getDate() - selectedValue);
    // Call the getAdminData method with the new date as an argument
    this.getAdminData(currentDate);
  }


  exportData(): void {
    // Get all the canvas elements by tag name
    // Get all the canvas elements by tag name
    const elements = document.getElementsByTagName('canvas');
    // Check if the elements exist
    if (elements) {
      // Create a new jsPDF object with the orientation, unit and format
      const pdf = new jsPDF('l', 'mm', 'a4');
      // Get the width and height of the PDF page
      const pageWidth = pdf.internal.pageSize.getWidth();
      const pageHeight = pdf.internal.pageSize.getHeight();
      // Define the number of rows and columns for the grid layout
      const rows = 2;
      const cols = 2;
      // Calculate the width and height of each chart
      const chartWidth = pageWidth / cols;
      const chartHeight = pageHeight / rows;
      // Loop through the elements
      for (let i = 0; i < elements.length; i++) {
        // Use html2canvas to convert the element to a canvas
        html2canvas(elements[i]).then((canvas) => {
          // Get the canvas data as an image
          const imgData = canvas.toDataURL('image/png');
          // Calculate the x and y position of the chart based on the grid layout
          const x = (i % cols) * chartWidth;
          const y = Math.floor(i / cols) * chartHeight;
          // Add the image to the PDF document at the calculated position and size
          pdf.addImage(imgData, 'PNG', x, y, chartWidth, chartHeight);
          // Save the PDF document as 'data.pdf' after the last element
          if (i === elements.length - 1) {
            pdf.save('data.pdf');
          }
        });
      }
    }



    // Get the data from the adminData variable as an array of arrays
    const data = [
      ['Total Interactions', 'Total Accounts', 'Total Posts', 'Average Interactions'],
      [
        this.adminData?.totalInteractions.toFixed(0),
        this.adminData?.totalAccountsInTimespan.toFixed(0),
        this.adminData?.totalPosts.toFixed(0),
        this.adminData?.averageInteractions.toFixed(0),
      ],
    ];

    // Create a CSV string from the data array
    let csv = '';
    data.forEach((row) => {
      csv += row.join(';');
      csv += '\n';
    });

    // Create a blob object from the CSV string with the type 'text/csv'
    const blob = new Blob([csv], { type: 'text/csv' });
    // Create a URL for the blob object
    const url = window.URL.createObjectURL(blob);
    // Create a link element with the URL and the download attribute
    const link = document.createElement('a');
    link.href = url;
    link.download = 'data.csv';
    // Append the link to the document body and click it
    document.body.appendChild(link);
    link.click();
    // Remove the link from the document body and revoke the URL
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

}
