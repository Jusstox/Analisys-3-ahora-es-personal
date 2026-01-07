<?php
include 'db_connect.php';

$run_id = $_POST['run_id'];

$win = $_POST['win']; 

$stmt = $conn->prepare("UPDATE Game_Runs SET Run_End = NOW(), Win = ? WHERE GameRun_ID = ?");
$stmt->bind_param("ii", $win, $run_id);

if ($stmt->execute()) {
    echo "Run ended successfully";
} else {
    echo "Error: " . $stmt->error;
}

$stmt->close();
$conn->close();
?>