import { useEffect, useState } from "react";
import {
  PaginatedTable,
  StreamViewButton,
  LoadingSpinner,
  SelectColumnFilter,
} from "components";

function StreamTable() {
  const columns = [
    {
      Header: "StreamTable",
      columns: [
        {
          Header: "Name",
          accessor: "name",
          disableFilters: true,
        },
        {
          Header: "No. Subjects",
          accessor: "subjectCount",
          Filter: SelectColumnFilter,
          filter: "equals",
        },
        {
          Header: "No. Consumers",
          accessor: "consumerCount",
          Filter: SelectColumnFilter,
          filter: "equals",
        },
        {
          Header: "No. Messages",
          accessor: "messageCount",
          disableFilters: true,
        },
        {
          Header: "Data",
          accessor: "data",
          disableFilters: true,
          Cell: (props: { row: any }) => {
            return <StreamViewButton content={props.row.values} />;
          },
        },
      ],
    },
  ];

  const [streams, setStreams] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getStreams();
  }, [!streams]);

  function getStreams() {
    fetch("/api/allStreams").then((res: any) => {
      if (res.ok) {
        res.json().then((data: any) => {
          setStreams(data);
          setLoading(false);
        });
      } else {
        alert(
          "An error occurred while fetching all streams: " + res.statusText
        );
      }
    });
  }

  return (
    <>
      {loading ? (
        <LoadingSpinner />
      ) : (
        <PaginatedTable columns={columns} data={streams} />
      )}
    </>
  );
}
export { StreamTable };
